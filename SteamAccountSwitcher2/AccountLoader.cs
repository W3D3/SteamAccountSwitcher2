using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuGet.Modules;
using Newtonsoft.Json;
using System.Windows;
using System.IO;

namespace SteamAccountSwitcher2
{
    class AccountLoader
    {
        Encryption encryptionType;
        string directory;
        const string basicPassword = "OQPTu9Rf4u4vkywWy+GCBptmXeqC0e456SR3N31vutU=";

        public AccountLoader(Encryption e)
        {
            encryptionType = e;
            this.directory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public AccountLoader(Encryption e, string directory)
        {
            encryptionType = e;
            this.directory = directory;
        }

        public List<SteamAccount> LoadBasicAccounts()
        {
            if(encryptionType == Encryption.Basic)
            {
                byte[] encrypted = File.ReadAllBytes(this.directory + "accounts.ini");
                //string encrypted = System.IO.File.ReadAllText(this.directory + "accounts.ini");
                //string output = JsonConvert.SerializeObject(product);
                //AesHelper.Encrypt()
               // MessageBox.Show(GetString(encrypted));
                string decrypted = GetString(AesHelper.Decrypt(encrypted, basicPassword));
               // MessageBox.Show(decrypted);
                List<SteamAccount> accountList = JsonConvert.DeserializeObject<List<SteamAccount>>(decrypted);
                return accountList;
            }
            else
            {
                throw new ArgumentException("Unsupported Encryption type!");
            }
        }

        public bool SaveBasicAccounts(List<SteamAccount> list)
        {
            if (encryptionType == Encryption.Basic)
            {
                string output = JsonConvert.SerializeObject(list);
                //MessageBox.Show(output);
                byte[] encrypted = AesHelper.Encrypt(GetBytes(output), basicPassword);

                File.WriteAllBytes(directory + "accounts.ini", encrypted);
            }
            else
            {
                throw new ArgumentException("Unsupported Encryption type is set!");
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
