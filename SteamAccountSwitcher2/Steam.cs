using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace SteamAccountSwitcher2
{
    class Steam
    {

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);


        const int SW_RESTORE = 9;

        string installDir;

        FileSystemWatcher watch;

        private Timer timer;

        public Steam(string installDir)
        {
            this.installDir = installDir;
        }

        public string InstallDir
        {
            get { return installDir; }
            set { installDir = value; }
        }

        public bool IsSteamRunning()
        {
            Process[] pname = Process.GetProcessesByName("steam");
            if (pname.Length == 0)
                return false;
            else
                return true;
        }

        public void KillSteam()
        {
            Process[] proc = Process.GetProcessesByName("steam");
            proc[0].Kill();
        }

        public void CleanKillSteam()
        {
            Process[] proc = Process.GetProcessesByName("steam");
            proc[0].CloseMainWindow();
            proc[0].Close();
        }

        public bool StartSteamAccount(SteamAccount acc)
        {
            bool finished = false;

            if (IsSteamRunning())
            {
                KillSteam();
            }

            while (finished == false)
            {
                if (IsSteamRunning() == false)
                {
                    Process p = new Process();
                    if (File.Exists(installDir))
                    {
                        p.StartInfo = new ProcessStartInfo(installDir, acc.getStartParameters());
                        p.Start();
                        finished = true;

                        return true;
                    }
                }
            }
            return false;
        }

        public bool StartSteamAccountSave(SteamAccount acc)
        {
            Process p;
            bool finished = false;
            string loginString = "-login " + acc.Username + " SAS-SAFEMODE";

            p = new Process();
            p.StartInfo = new ProcessStartInfo(installDir, "-fs_log " + loginString);

            if (IsSteamRunning())
            {
                CleanKillSteam();
            }

            while (finished == false)
            {
                if (IsSteamRunning() == false)
                {
                    p.Start();
                    finished = true;

                    System.Threading.Thread.Sleep(5000);
                    bool steamNotUpdating = false;
                    while(steamNotUpdating == false)
                    {
                        steamNotUpdating = CheckLogFile();
                    }

                    if (steamNotUpdating)
                    {
                        try
                        {
                            Debug.WriteLine("Starting input manager!");
                            System.Threading.Thread.Sleep(500);
                            
                            IntPtr handle = p.MainWindowHandle;
                            if (IsIconic(handle))
                            {
                                ShowWindow(handle, SW_RESTORE);
                            }
                            SetForegroundWindow(handle);
                            //Clipboard.SetText(acc.Username);
                            InputSimulator s = new InputSimulator();
                            //s.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

                            //s.Keyboard.TextEntry(acc.Username);
                            //System.Threading.Thread.Sleep(100);
                            //s.Keyboard.KeyDown(VirtualKeyCode.TAB);
                            //s.Keyboard.KeyUp(VirtualKeyCode.TAB);
                            //System.Threading.Thread.Sleep(100);
                            s.Keyboard.TextEntry(acc.Password);
                            System.Threading.Thread.Sleep(100);
                            s.Keyboard.KeyDown(VirtualKeyCode.RETURN);

                            return true;
                        }
                        catch
                        {
                            MessageBox.Show("Error logging in. Steam not in foreground.");
                        }
                        //MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }                    
                }
            }
            return false;
        }

        public void Watch()
        {
            string logDir = installDir.Replace("Steam.exe","logs\\");

            watch = new FileSystemWatcher();
            watch.Path = logDir;
            watch.Filter = "bootstrap_log.txt";
            watch.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.Size; //more options
            //watch.Changed += new FileSystemEventHandler(OnChanged);
            //watch.EnableRaisingEvents = true;
        }

        /// Functions:
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //if (!FileIsReady(e.FullPath)) return; //first notification the file is arriving

            using (var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    MessageBox.Show(reader.ReadLine());
                }
            }
            /*
            string logDir = installDir.Replace("Steam.exe", "logs\\");
            if (e.FullPath == logDir + "bootstrap_log.txt")
            {
                // do stuff
                List<string> text = File.ReadLines(e.FullPath).Reverse().Take(1).ToList();
                MessageBox.Show(text[0].ToString());
            }*/
        }

        private bool CheckLogFile()
        {
            string logDir = installDir.Replace("Steam.exe", "logs\\");
            string filename = logDir + "bootstrap_log.txt";

            using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // Seek 1024 bytes from the end of the file
                fs.Seek(-512, SeekOrigin.End);
                // read 1024 bytes
                byte[] bytes = new byte[512];
                fs.Read(bytes, 0, 512);
                // Convert bytes to string
                string s = Encoding.Default.GetString(bytes);
                // or string s = Encoding.UTF8.GetString(bytes);
                // and output to console
                //Debug.WriteLine(s);
                string[] splitter = new string[1];
                splitter[0] = "Startup - updater";
                string[] parts = s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                bool steamDone = parts[parts.Length - 1].Contains("Background update loop checking for update.");
                Debug.WriteLineIf(steamDone, "steam is Done.");
                return steamDone;
            }
        }

        public bool LogoutSteam()
        {
            Process p = new Process();
            if (File.Exists(installDir))
            {
                p.StartInfo = new ProcessStartInfo(installDir, "-shutdown");
                p.Start();
                return true;
            }
            return false;

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
