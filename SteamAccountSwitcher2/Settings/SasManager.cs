using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SteamAccountSwitcher2
{
    public sealed class SasManager
    {
        private Steam steamInstance;
        ObservableCollection<SteamAccount> accountList = new ObservableCollection<SteamAccount>();

        AccountLoader loader;
        bool autosaveAccounts = true;
        private SteamStatus steamStatus;

        private SasManager()
        {
            steamInstance = new Steam(Properties.Settings.Default.steamInstallDir);
            steamStatus = new SteamStatus();
        }

        private static SasManager instance = null;

        public static SasManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SasManager();
                }

                return instance;
            }
        }

        public Properties.Settings Settings
        {
            get => Properties.Settings.Default;
        }

        public Steam SteamInstance => steamInstance;

        public SteamStatus SteamStatus => steamStatus;

        public ObservableCollection<SteamAccount> AccountList => accountList;

        public void setSteamInstallDir(string installDir)
        {
            if (installDir != null)
            {
                Properties.Settings.Default.steamInstallDir = installDir;
                this.steamInstance = new Steam(installDir);
            }
        }

        public void initializeAccountsFromFile()
        {
            loader = new AccountLoader(EncryptionType.Basic);

            if (loader.AccountFileExists())
            {
                accountList = new ObservableCollection<SteamAccount>(loader.LoadBasicAccounts());
            }
            else
            {
                accountList = new ObservableCollection<SteamAccount>();
            }
        }

        public void saveOnExit()
        {
            //User has exited the application, save all data
            if (autosaveAccounts)
            {
                loader.SaveBasicAccounts(accountList.ToList<SteamAccount>());
            }
        }

        public void startSteamWithAccount(SteamAccount selectedAcc)
        {
            try
            {
                if (Properties.Settings.Default.safemode)
                {
                    steamInstance.StartSteamAccountSafe(selectedAcc);
                    Mouse.OverrideCursor = Cursors.Wait;
                }
                else
                {
                    steamInstance.StartSteamAccount(selectedAcc);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public void setAutoStart(bool autostart)
        {
            Debug.WriteLine(Environment.SpecialFolder.Startup);
            Settings.autostart = autostart;
            if (autostart)
            {
                try
                {
                    Microsoft.Win32.RegistryKey key =
                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                            true);
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                catch
                {
                    Debug.WriteLine("Failed to set autostart");
                }
            }
            else
            {
                try
                {
                    Microsoft.Win32.RegistryKey key =
                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                            true);
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    key.DeleteValue(curAssembly.GetName().Name);
                    //key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                catch
                {
                    Debug.WriteLine("Failed to remove autostart");
                }
            }
        }
    }
}