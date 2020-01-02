using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Gameloop.Vdf;
using Microsoft.Win32;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Manages everything related to cached steam accounts
    /// </summary>
    class CachedAccountManager
    {
        private Steam _steamInstallation;
        List<SteamAccount> _cachedAccounts = new List<SteamAccount>();
        private string loginUsersVDFPath;

        public CachedAccountManager(Steam installation)
        {
            _steamInstallation = installation;
            loginUsersVDFPath = Path.Combine(installation.InstallDir, "config/loginusers.vdf");
        }

        public IEnumerable<SteamAccount> CachedAccounts => _cachedAccounts;

        public void scanForAccounts()
        {
            dynamic loginUsersVdf = VdfConvert.Deserialize(File.ReadAllText(loginUsersVDFPath));
            // 'volvo' is a VProperty, analogous to Json.NET's JProperty
            _cachedAccounts.Clear();
            foreach (var account in loginUsersVdf.Value)
            {
                Console.WriteLine(account.Value.AccountName);
                MessageBox.Show(account.Key.ToString());
                _cachedAccounts.Add(new SteamAccount(
                        (string) account.Key.ToString(),
                        (string) account.Value.AccountName.Value.ToString(),
                        (string) account.Value.PersonaName.Value.ToString(),
                        account.Value.RememberPassword.Value.ToString() == "1",
                        account.Value.mostrecent.Value.ToString() == "1",
                        long.Parse(account.Value.Timestamp.Value.ToString())
                ));
            }

            // Now do whatever with this
            Console.WriteLine(_cachedAccounts.Count); // Prints 3
        }

        public void setActiveAccount(SteamAccount selectedAccount)
        {
            _steamInstallation.KillSteam();
            dynamic loginUsersVdf = VdfConvert.Deserialize(File.ReadAllText(loginUsersVDFPath));
            try
            {
                foreach (var account in loginUsersVdf.Value)
                {
                    if (account.Key.ToString() == selectedAccount.SteamId)
                    {
                        account.Value.mostrecent.Value = "1";
                        if (account.Value.RememberPassword.Value == "0")
                        {
                            MessageBox.Show(
                                "This account does not have a password associated with it and can only be started if it has already logged in once and 'Remember Password' has been checked",
                                "Cannot start with account", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    else
                    {
                        account.Value.mostrecent.Value = "0";
                    }
                }

                MessageBox.Show(loginUsersVdf.ToString());
                File.WriteAllText(loginUsersVDFPath, loginUsersVdf.ToString());

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Valve\Steam"))
                {
                    key.SetValue("AutoLoginUser", selectedAccount.AccountName);
                    key.SetValue("RememberPassword", 1);
                }
                _steamInstallation.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
