using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using SteamAccountSwitcher2.Encryption;

namespace SteamAccountSwitcher2
{
    public class AccountLoader
    {
        private readonly string _directory;

        private const string BasicKey = "OQPTu9Rf4u4vkywWy+GCBptmXeqC0e456SR3N31vutU=";

        public AccountLoader(EncryptionType e)
        {
            EncryptionType = e;
            _directory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public EncryptionType EncryptionType { get; set; }

        /// <summary>
        /// Password that is used only when Encryption Type is set to Password!
        /// </summary>
        public string Password { get; set; }

        public string AccountsFilePath => Path.Combine(_directory, "accounts.ini");

        public List<SteamAccount> LoadAccounts()
        {
            var retry = true;
            while (retry)
            {
                string encryptionKey;
                switch (EncryptionType)
                {
                    case EncryptionType.Basic:
                        encryptionKey = BasicKey;
                        break;
                    case EncryptionType.Password:
                        if (!string.IsNullOrEmpty(Password))
                        {
                            encryptionKey = Password;
                        }
                        else
                        {
                            Password = AskForPassword();
                            encryptionKey = Password;
                        }

                        break;
                    default:
                        throw new ArgumentException("Unsupported EncryptionType type!");
                }

                try
                {
                    var encrypted = File.ReadAllText(_directory + "accounts.ini");
                    var decrypted = EncryptionHelper.Decrypt(encrypted, encryptionKey);
                    var accountList = JsonConvert.DeserializeObject<List<SteamAccount>>(decrypted);
                    return accountList;
                }
                catch (CryptographicException)
                {
                    MessageBox.Show("Try entering the password again.", "Could not decrypt");
                    Password = null;
                }
                catch (JsonException e)
                {
                    MessageBox.Show(e.Message, "Fatal Error when reading accounts file!");
                    retry = false;
                }
                catch (Exception)
                {
                    retry = false;
                }
            }

            return null;
        }

        private string AskForPassword()
        {
            var passwordWindow = new PasswordWindow(false);
            passwordWindow.ShowDialog();
            if (passwordWindow.Password == null)
            {
                Environment.Exit(1);
            }

            return passwordWindow.Password;
        }

        public void SaveAccounts(List<SteamAccount> list)
        {
            string encryptionKey;
            switch (EncryptionType)
            {
                case EncryptionType.Basic:
                    encryptionKey = BasicKey;
                    break;
                case EncryptionType.Password:
                    encryptionKey = Password;
                    break;
                default:
                    throw new ArgumentException("Unsupported EncryptionType type!");
            }


            var output = JsonConvert.SerializeObject(list, Formatting.None);
            var encrypted = EncryptionHelper.Encrypt(output, encryptionKey);

            File.WriteAllText(AccountsFilePath, encrypted);
        }

        public bool AccountFileExists()
        {
            return File.Exists(AccountsFilePath);
        }
    }
}