using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SteamAccountSwitcher2
{
    public class Steam
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        private const int SW_RESTORE = 9;

        private string _pathToSteamExe;

        public Steam(string pathToSteamExe)
        {
            PathToSteamExe = pathToSteamExe;
        }

        public string PathToSteamExe
        {
            get { return _pathToSteamExe; }
            set {
                if (string.IsNullOrEmpty(value) || !value.EndsWith("steam.exe"))
                    throw new ArgumentException("Invalid Steam Path: " + value);

                _pathToSteamExe = value; }
        }

        public string PathToSteamInstallationFolder => _pathToSteamExe.ToLower().Replace("steam.exe", "");

        public bool IsSteamRunning()
        {
            var steamProcesses = Process.GetProcessesByName("steam");
            return steamProcesses.Length > 0;
        }

        public void KillSteam()
        {
            var steamProcesses = Process.GetProcessesByName("steam");
            if (steamProcesses.Length > 0) 
                steamProcesses[0].Kill();
        }

        public void CleanKillSteam()
        {
            var proc = Process.GetProcessesByName("steam");
            if (proc.Length > 0)
            {
                proc[0].CloseMainWindow();
                proc[0].Close();
            }
        }

        public void Start()
        {
            var p = new Process();
            if (File.Exists(_pathToSteamExe))
            {
                p.StartInfo = new ProcessStartInfo(_pathToSteamExe);
                p.Start();
            }
        }

        public bool StartSteamAccount(SteamAccount acc)
        {
            var finished = false;

            if (IsSteamRunning()) CleanKillSteam();

            var waitTimer = 30;
            while (finished == false)
            {
                if (waitTimer == 0)
                {
                    KillSteam();
                    Debug.WriteLine("Hard killed steam.");
                }

                if (IsSteamRunning() == false)
                {
                    var p = new Process();
                    if (File.Exists(_pathToSteamExe))
                    {
                        p.StartInfo = new ProcessStartInfo(_pathToSteamExe, acc.StartParameters());
                        p.Start();
                        finished = true;

                        return true;
                    }
                }

                Thread.Sleep(100);
                waitTimer--;
            }

            return false;
        }

        [Obsolete("IsSteamReady is deprecated.")]
        private bool IsSteamReady()
        {
            var logDir = _pathToSteamExe.Replace("Steam.exe", "logs\\");
            var filename = logDir + "bootstrap_log.txt";

            using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // Seek 1024 bytes from the end of the file
                fs.Seek(-512, SeekOrigin.End);
                // read 1024 bytes
                var bytes = new byte[512];
                fs.Read(bytes, 0, 512);
                // Convert bytes to string
                var s = Encoding.Default.GetString(bytes);
                // or string s = Encoding.UTF8.GetString(bytes);
                // and output to console
                //Debug.WriteLine(s);
                var splitter = new string[1];
                splitter[0] = "Startup - updater";
                var parts = s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                var steamDone = parts[parts.Length - 1].Contains("Background update loop checking for update.");
                Debug.WriteLineIf(steamDone, "steam is Done.");
                return steamDone;
            }
        }

        public bool LogoutSteam()
        {
            var p = new Process();
            if (File.Exists(_pathToSteamExe))
            {
                p.StartInfo = new ProcessStartInfo(_pathToSteamExe, "-shutdown");
                p.Start();
                return true;
            }

            return false;
        }
    }
}