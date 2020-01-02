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
        string directory;

        const string basicPassword = "OQPTu9Rf4u4vkywWy+GCBptmXeqC0e456SR3N31vutU=";

        public AccountLoader(EncryptionType e)
        {
            _encryptionType = e;
            this.directory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public AccountLoader(EncryptionType e, string directory)
        {
            _encryptionType = e;
            this.directory = directory;
        }

        public List<SteamAccount> LoadBasicAccounts()
        {

            if (_encryptionType == EncryptionType.Basic)
            {
                try
                {
                    byte[] encrypted = File.ReadAllBytes(this.directory + "accounts.ini");
                    string decrypted = GetString(AesHelper.Decrypt(encrypted, basicPassword));
                    List<SteamAccount> accountList = JsonConvert.DeserializeObject<List<SteamAccount>>(decrypted);
                    return accountList;
                }
                catch(Exception e)
                {
                    throw new ApplicationException("Fatal Error when reading accounts file!");
                }
            }
            else
            {
                throw new ArgumentException("Unsupported EncryptionType type!");
            }
        }

        public bool SaveBasicAccounts(List<SteamAccount> list)
        {
            if (_encryptionType == EncryptionType.Basic)
            {
                string output = JsonConvert.SerializeObject(list, Formatting.None);
                //MessageBox.Show(output);
                byte[] encrypted = AesHelper.Encrypt(GetBytes(output), basicPassword);

                File.WriteAllBytes(directory + "accounts.ini", encrypted);
            }
            else
            {
                throw new ArgumentException("Unsupported EncryptionType type is set!");
            }
            return false;
        }

        public bool AccountFileExists()
        {
            return File.Exists(directory + "accounts.ini");
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
