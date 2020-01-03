using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using SteamAccountSwitcher2.Settings;

namespace SteamAccountSwitcher2
{
    public sealed class SasManager
    {
        private ObservableCollection<SteamAccount> _accountList = new ObservableCollection<SteamAccount>();

        private Steam _steamInstallation;
        private CachedAccountManager _cachedAccountManager;
        private UserSettings _userSettings;
        private AccountLoader loader;
        private SteamStatus _steamStatus;

        bool autosaveAccounts = true;

        private SasManager()
        {
            _userSettings = new UserSettings(true);
            _steamInstallation = new Steam(UserSettings.SteamInstallDir);

            _cachedAccountManager = new CachedAccountManager(_steamInstallation);
            _steamStatus = new SteamStatus();
        }

        private static SasManager _instance = null;

        public static SasManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SasManager();
                }

                return _instance;
            }
        }

        public Steam SteamInstallation => _steamInstallation;

        public SteamStatus SteamStatus => _steamStatus;

        public ObservableCollection<SteamAccount> AccountList => _accountList;

        public UserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }

        public void SetSteamInstallDir(string installDir)
        {
            if (installDir != null)
            {
                Properties.Settings.Default.steamInstallDir = installDir;
                _steamInstallation = new Steam(installDir);
            }
        }

        public void InitializeAccountsFromFile()
        {
            loader = new AccountLoader(UserSettings.EcryptionType);

            if (loader.AccountFileExists())
            {
                _accountList = new ObservableCollection<SteamAccount>(loader.LoadAccounts());
            }
            else
            {
                _accountList = new ObservableCollection<SteamAccount>();
            }
        }

        public void SaveAccounts()
        {
            //User has exited the application, save all accounts
            if (autosaveAccounts)
            {
                loader.SaveAccounts(_accountList.ToList());
            }
        }

        public void StartSteamWithAccount(SteamAccount selectedAcc)
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
                    _steamInstallation.StartSteamAccount(selectedAcc);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error when starting account", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetAutoStart(bool autostart)
        {
            Properties.Settings.Default.autostart = autostart;

            try
            {
                Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                if (autostart)
                {
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                else
                {
                    key.DeleteValue(curAssembly.GetName().Name);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to set autostart to " + autostart);
            }
        }

        public void ScanAndAddAccounts()
        {
            _cachedAccountManager.scanForAccounts();

            foreach (var scannerAccount in _cachedAccountManager.CachedAccounts)
            {
                if (!_accountList.Contains(scannerAccount))
                    _accountList.Add(scannerAccount);
            }
        }

        public void ApplyUserSettings(UserSettings newSettings)
        {
            if (newSettings.Autostart != _userSettings.Autostart)
            {
                SetAutoStart(newSettings.Autostart);
            }

            if (newSettings.SteamInstallDir != _userSettings.SteamInstallDir)
            {
                SetSteamInstallDir(newSettings.SteamInstallDir);
            }

            if (newSettings.EcryptionType != _userSettings.EcryptionType)
            {
                SetEncryption(newSettings.EcryptionType);
            }

            _userSettings = newSettings;
        }

        private void SetEncryption(EncryptionType newEcryption)
        {
            //loader
        }
    }
}