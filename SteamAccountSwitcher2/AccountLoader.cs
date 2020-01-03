using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using SteamAccountSwitcher2.Encryption;

namespace SteamAccountSwitcher2
{
    class AccountLoader
    {
        EncryptionType _encryptionType;
        string _directory;
        private string _password;

        const string BasicKey = "OQPTu9Rf4u4vkywWy+GCBptmXeqC0e456SR3N31vutU=";

        public AccountLoader(EncryptionType e)
        {
            _encryptionType = e;
            this._directory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public AccountLoader(EncryptionType e, string directory)
        {
            _encryptionType = e;
            this._directory = directory;
        }

        public EncryptionType EncryptionType
        {
            get => _encryptionType;
            set => _encryptionType = value;
        }

        /// <summary>
        /// Password that is used only when Encryption Type is set to Password!
        /// </summary>
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string AccountsFilePath => Path.Combine(_directory, "accounts.ini");

        public List<SteamAccount> LoadAccounts()
        {
            bool retry = true;
            while (retry)
            {
                string encryptionKey;
                switch (_encryptionType)
                {
                    case EncryptionType.Basic:
                        encryptionKey = BasicKey;
                        break;
                    case EncryptionType.Password:
                        if (!string.IsNullOrEmpty(_password))
                        {
                            encryptionKey = _password;
                        }
                        else
                        {
                            _password = AskForPassword();
                            encryptionKey = _password;
                        }
                        break;
                    default:
                        throw new ArgumentException("Unsupported EncryptionType type!");
                }

                try
                {
                    string encrypted = File.ReadAllText(this._directory + "accounts.ini");
                    string decrypted = EncryptionHelper.Decrypt(encrypted, encryptionKey);
                    List<SteamAccount> accountList = JsonConvert.DeserializeObject<List<SteamAccount>>(decrypted);
                    return accountList;
                }
                catch (CryptographicException e)
                {
                    MessageBox.Show("Try entering the password again.", "Could not decrypt");
                    _password = null;
                }
                catch (JsonException e)
                {
                    MessageBox.Show(e.Message, "Fatal Error when reading accounts file!");
                    retry = false;
                }
                catch (Exception e)
                {
                    retry = false;
                }
            }

            return null;
        }

        private string AskForPassword()
        {
            PasswordWindow passwordWindow = new PasswordWindow(false);
            passwordWindow.ShowDialog();
            if (passwordWindow.Password == null)
            {
                System.Environment.Exit(1);
            }
            return passwordWindow.Password;
        }

        public void SaveAccounts(List<SteamAccount> list)
        {
            string encryptionKey;
            switch (_encryptionType)
            {
                case EncryptionType.Basic:
                    encryptionKey = BasicKey;
                    break;
                case EncryptionType.Password:
                    encryptionKey = _password;
                    break;
                default:
                    throw new ArgumentException("Unsupported EncryptionType type!");
            }


            string output = JsonConvert.SerializeObject(list, Formatting.None);
            string encrypted = EncryptionHelper.Encrypt(output, encryptionKey);

            File.WriteAllText(AccountsFilePath, encrypted);
        }

        public bool AccountFileExists()
        {
            return File.Exists(AccountsFilePath);
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}