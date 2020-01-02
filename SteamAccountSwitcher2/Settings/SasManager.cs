using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SteamAccountSwitcher2
{
    public sealed class SasManager
    {
        private Steam steamInstance;
        ObservableCollection<SteamAccount> accountList = new ObservableCollection<SteamAccount>();
        CachedAccountManager _cachedAccountManager;

        AccountLoader loader;
        bool autosaveAccounts = true;
        private SteamStatus steamStatus;

        private SasManager()
        {
            steamInstance = new Steam(Properties.Settings.Default.steamInstallDir);

            _cachedAccountManager = new CachedAccountManager(steamInstance);
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
                if (selectedAcc.CachedAccount)
                {
                    // If no password login is possible / needed
                    _cachedAccountManager.startCachedAccount(selectedAcc);
                }
                else
                {
                    _cachedAccountManager.resetActiveAccount();
                    steamInstance.StartSteamAccount(selectedAcc);
                }

                /*if (Properties.Settings.Default.safemode)
                {
                    steamInstance.StartSteamAccountSafe(selectedAcc);
                    Mouse.OverrideCursor = Cursors.Wait;
                }
                else
                {
                    steamInstance.StartSteamAccount(selectedAcc);
                }*/
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public void setAutoStart(bool autostart)
        {
            Debug.WriteLine(Environment.SpecialFolder.Startup);
            Properties.Settings.Default.autostart = autostart;
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
                }
                catch
                {
                    Debug.WriteLine("Failed to remove autostart");
                }
            }
        }

        public void ScanAndAddAccounts()
        {
            _cachedAccountManager.scanForAccounts();

            foreach (var scannerAccount in _cachedAccountManager.CachedAccounts)
            {
                if (!accountList.Contains(scannerAccount))
                    accountList.Add(scannerAccount);
            }
        }
    }
}