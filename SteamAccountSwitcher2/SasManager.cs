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
        private UserSettings _globalSettings;
        private AccountLoader loader;
        private SteamStatus _steamStatus;

        bool autosaveAccounts = true;

        private SasManager()
        {
            _globalSettings = new UserSettings(true);
            _steamInstallation = new Steam(GlobalSettings.SteamInstallDir);

            _cachedAccountManager = new CachedAccountManager(_steamInstallation);
            _steamStatus = new SteamStatus();
        }
        private SasManager(string installDir)
        {
            _globalSettings = new UserSettings(true);
            _steamInstallation = new Steam(installDir);
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

        public static void InitiateInstanceWithDir(string installDir)
        {
            if (installDir == null)
            {
                throw new ArgumentException("installDir cannot be empty or null!");
            }
            if (_instance == null)
            {
                _instance = new SasManager(installDir);
            }
            else
                _instance.SetSteamInstallDir(installDir);

        }

        public Steam SteamInstallation => _steamInstallation;

        public SteamStatus SteamStatus => _steamStatus;

        public ObservableCollection<SteamAccount> AccountList => _accountList;

        public UserSettings GlobalSettings
        {
            get => _globalSettings
                .Copy(); // Only give copies so no accidental global changes can be made, copy is non global
        }

        public void SetSteamInstallDir(string pathToSteamExe)
        {
            if (string.IsNullOrEmpty(pathToSteamExe) || !pathToSteamExe.EndsWith("steam.exe"))
                throw new ArgumentException("Invalid Steam Path: " + pathToSteamExe);

            if (pathToSteamExe != null)
            {
                _steamInstallation = new Steam(pathToSteamExe);
                _cachedAccountManager = new CachedAccountManager(_steamInstallation);
                _globalSettings.SteamInstallDir = _steamInstallation.PathToSteamInstallationFolder;
            }
        }

        public void InitializeAccountsFromFile()
        {
            loader = new AccountLoader(GlobalSettings.EcryptionType);

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

                // Success
                _globalSettings.Autostart = autostart;
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
            if (newSettings.Autostart != _globalSettings.Autostart)
            {
                SetAutoStart(newSettings.Autostart);
            }

            if (newSettings.SteamInstallDir != _globalSettings.SteamInstallDir)
            {
                SetSteamInstallDir(newSettings.SteamInstallDir);
            }

            if (newSettings.EcryptionType != _globalSettings.EcryptionType)
            {
                SetEncryption(newSettings.EcryptionType);
            }
        }

        public void SetEncryption(EncryptionType newEcryptionType)
        {
            switch (newEcryptionType)
            {
                case EncryptionType.Password:
                    PasswordWindow passwordWindow = new PasswordWindow(true);
                    passwordWindow.ShowDialog();
                    var password = passwordWindow.Password;
                    if (string.IsNullOrEmpty(password))
                    {
                        Debug.WriteLine("Will not change encryption to empty password");
                        return;
                    }

                    loader.Password = password;
                    break;
                case EncryptionType.Basic:
                    loader.Password = null;
                    break;
            }

            loader.EncryptionType = newEcryptionType;
            loader.SaveAccounts(AccountList.ToList());
            _globalSettings.EcryptionType = newEcryptionType;
        }
    }
}