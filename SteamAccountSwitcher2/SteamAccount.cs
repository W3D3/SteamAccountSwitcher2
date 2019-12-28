using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher2
{
    public class SteamAccount
    {
        private string _name;
        private string _username;
        private string _password;
        private AccountType _type;

        private const string ImageFolder = "images";

        public SteamAccount()
        {

        }

        public SteamAccount(string username, string password)
        {
            this._name = username;
            this._username = username;
            this._password = password;
            this._type = AccountType.Main;
        }

        public string Name
        {
            get { return _name; }
            set { this._name = value; }
        }

        public string Username
        {
            get { return _username; }
            set { this._username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { this._password = value; }
        }

        public AccountType Type
        {
            get { return _type; }
            set { this._type = value; }
        }

        public string ImageSource
        {
            get
            {
                return ImageFolder + "/acc-bg-" + _type.ToString().ToLower() + ".jpg";
            }
        }

        public string getStartParameters()
        {
            return "-login " + this._username + " " + this._password;
        }
        public override string ToString()
        {
            return _name + "~ (user: " + _username + ")";
        }
    }
}
