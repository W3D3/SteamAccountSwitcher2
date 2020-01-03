using Newtonsoft.Json;
using NuGet.Modules;
using System;
using System.Collections.Generic;
using System.IO;

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

            try
            {
                byte[] encrypted = File.ReadAllBytes(this._directory + "accounts.ini");
                string decrypted = GetString(AesHelper.Decrypt(encrypted, encryptionKey));
                List<SteamAccount> accountList = JsonConvert.DeserializeObject<List<SteamAccount>>(decrypted);
                return accountList;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Fatal Error when reading accounts file!", e);
            }
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
            byte[] encrypted = AesHelper.Encrypt(GetBytes(output), encryptionKey);

            File.WriteAllBytes(AccountsFilePath, encrypted);
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