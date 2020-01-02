using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Gameloop.Vdf;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Scans already logged in accounts
    /// </summary>
    class AccountScanner
    {
        List<CachedAccount> cachedAccounts = new List<CachedAccount>();
        private string loginUsersVDFPath;

        public AccountScanner(Steam installation)
        {
            loginUsersVDFPath = Path.Combine(installation.InstallDir, "config/loginusers.vdf");
        }

        public IEnumerable<SteamAccount> Accounts => cachedAccounts;

        public void scanForAccounts()
        {
            dynamic volvo = VdfConvert.Deserialize(File.ReadAllText(loginUsersVDFPath));
            // 'volvo' is a VProperty, analogous to Json.NET's JProperty
            cachedAccounts.Clear();
            foreach (var account in volvo.Value)
            {
                Console.WriteLine(account.Value.AccountName);
                MessageBox.Show(account.Key.ToString());
                cachedAccounts.Add(new CachedAccount(
                        (string) account.Key.ToString(),
                        (string) account.Value.AccountName.Value.ToString(),
                        (string) account.Value.PersonaName.Value.ToString(),
                        account.Value.RememberPassword.Value.ToString() == "1",
                        account.Value.mostrecent.Value.ToString() == "1",
                        long.Parse(account.Value.Timestamp.Value.ToString())
                ));
            }

            // Now do whatever with this
            Console.WriteLine(cachedAccounts.Count); // Prints 3
        }
    }
}
