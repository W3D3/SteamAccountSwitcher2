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
        private CachedAccountManager _cachedAccountManager;

        private UserSettings _globalSettings;

        private AccountLoader _loader;

        bool autosaveAccounts = true;

        public Steam SteamInstallation { get; private set; }

        public SteamStatus SteamStatus { get; }

        public ObservableCollection<SteamAccount> AccountList { get; private set; } = new ObservableCollection<SteamAccount>();

        public UserSettings GlobalSettings =>  _globalSettings.Copy(); // Only give copies so no accidental global changes can be made, copy is non global

        private SasManager()
        {
            _globalSettings = new UserSettings(true);
            SteamInstallation = new Steam(GlobalSettings.SteamInstallDir);

            _cachedAccountManager = new CachedAccountManager(SteamInstallation);
            SteamStatus = new SteamStatus();
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
        
        public void SetSteamInstallDir(string installDir)
        {
            if (installDir != null)
            {
                SteamInstallation = new Steam(installDir);
                _globalSettings.SteamInstallDir = installDir;
            }
        }

        public void InitializeAccountsFromFile()
        {
            _loader = new AccountLoader(GlobalSettings.EncryptionType);

            if (_loader.AccountFileExists())
            {
                AccountList = new ObservableCollection<SteamAccount>(_loader.LoadAccounts());
            }
            else
            {
                AccountList = new ObservableCollection<SteamAccount>();
            }
        }

        public void SaveAccounts()
        {
            //User has exited the application, save all accounts
            if (autosaveAccounts)
            {
                _loader.SaveAccounts(AccountList.ToList());
            }
        }

        public void StartSteamWithAccount(SteamAccount selectedAcc)
        {
            try
            {
                if (selectedAcc.CachedAccount)
                {
                    // If no password login is possible / needed
                    _cachedAccountManager.StartCachedAccount(selectedAcc);
                }
                else
                {
                    _cachedAccountManager.ResetActiveAccount();
                    SteamInstallation.StartSteamAccount(selectedAcc);
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
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);
                var curAssembly = Assembly.GetExecutingAssembly();
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
            _cachedAccountManager.ScanForAccounts();

            foreach (var scannerAccount in _cachedAccountManager.CachedAccounts)
            {
                if (!AccountList.Contains(scannerAccount))
                    AccountList.Add(scannerAccount);
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

            if (newSettings.EncryptionType != _globalSettings.EncryptionType)
            {
                SetEncryption(newSettings.EncryptionType);
            }
        }

        public void SetEncryption(EncryptionType newEncryptionType)
        {
            switch (newEncryptionType)
            {
                case EncryptionType.Password:
                    var passwordWindow = new PasswordWindow(true);
                    passwordWindow.ShowDialog();
                    var password = passwordWindow.Password;
                    if (string.IsNullOrEmpty(password))
                    {
                        Debug.WriteLine("Will not change encryption to empty password");
                        return;
                    }

                    _loader.Password = password;
                    break;
                case EncryptionType.Basic:
                    _loader.Password = null;
                    break;
            }

            _loader.EncryptionType = newEncryptionType;
            _loader.SaveAccounts(AccountList.ToList());
            _globalSettings.EncryptionType = newEncryptionType;
        }
    }
}