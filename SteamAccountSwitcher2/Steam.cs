using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            p = new Process();
            p.StartInfo = new ProcessStartInfo(installDir, "-fs_log");

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
                    try { 
                    IntPtr handle = p.MainWindowHandle;
                    if (IsIconic(handle))
                    {
                        ShowWindow(handle, SW_RESTORE);
                    }
                    SetForegroundWindow(handle);
                    //Clipboard.SetText(acc.Username);
                    InputSimulator s = new InputSimulator();
                    //s.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

                    s.Keyboard.TextEntry(acc.Username);
                    System.Threading.Thread.Sleep(100);
                    s.Keyboard.KeyDown(VirtualKeyCode.TAB);
                    s.Keyboard.KeyUp(VirtualKeyCode.TAB);
                    System.Threading.Thread.Sleep(100);
                    s.Keyboard.TextEntry(acc.Password);
                    }
                    catch
                    {
                        MessageBox.Show("Error logging in. Steam not in foreground.");
                    }
                    //MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    return true;
                    
                }
            }
            return false;
        }

        public void Watch()
        {
            string logDir = installDir.Replace("Steam.exe","logs\\");

            var watch = new FileSystemWatcher();
            watch.Path = logDir;
            watch.Filter = "bootstrap_log.txt";
            watch.NotifyFilter = NotifyFilters.Size; //more options
            watch.Changed += new FileSystemEventHandler(OnChanged);
            watch.EnableRaisingEvents = true;
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

            string logDir = installDir.Replace("Steam.exe", "logs\\");
            if (e.FullPath == logDir + "bootstrap_log.txt")
            {
                // do stuff
                List<string> text = File.ReadLines(e.FullPath).Reverse().Take(1).ToList();
                MessageBox.Show(text[0].ToString());
            }
        }

        private bool FileIsReady(string path)
        {
            //One exception per file rather than several like in the polling pattern
            try
            {
                //If we can't open the file, it's still copying
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
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

        private static class SendKeys
        {
            /// <summary>
            ///   Sends the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            public static void Send(Key key)
            {
                if (Keyboard.PrimaryDevice != null)
                {
                    if (Keyboard.PrimaryDevice.ActiveSource != null)
                    {
                        var e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Down) { RoutedEvent = Keyboard.KeyDownEvent };
                        InputManager.Current.ProcessInput(e1);
                    }
                }
            }
        }
    }
}
