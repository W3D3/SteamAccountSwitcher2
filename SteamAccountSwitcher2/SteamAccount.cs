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
        private string _accountName;
        private string _password;
        private AccountType _type;
        private string _steamId;
        private string _personaName;
        private bool _rememberPassword;
        private bool _mostrecent;
        private long _timestamp;

        private const string ImageFolder = "images";

        public SteamAccount()
        {
        }

        public SteamAccount(string accountName, string password)
        {
            this._name = accountName;
            this._accountName = accountName;
            this._password = password;
            this._type = AccountType.Main;
        }

        public SteamAccount(string steamId, string accountName, string personaName, bool rememberPassword,
            bool mostrecent, long timestamp)
        {
            SteamId = steamId;
            AccountName = accountName;
            PersonaName = personaName;
            Name = PersonaName; // For UI
            RememberPassword = rememberPassword;
            Mostrecent = mostrecent;
            Timestamp = timestamp;
            this._type = AccountType.Main;
        }

        public string SteamId
        {
            get => _steamId;
            set => _steamId = value;
        }

        public string AccountName
        {
            get => _accountName;
            set => _accountName = value;
        }

        public string PersonaName
        {
            get => _personaName;
            set => _personaName = value;
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => _rememberPassword = value;
        }

        public bool Mostrecent
        {
            get => _mostrecent;
            set => _mostrecent = value;
        }

        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        public string Name
        {
            get => _name;
            set => this._name = value;
        }

        public string Password
        {
            get => _password;
            set => this._password = value;
        }

        public AccountType Type
        {
            get => _type;
            set => this._type = value;
        }

        public string BGImage => ImageFolder + "/acc-bg-" + _type.ToString().ToLower() + ".jpg";

        public bool IsCached => string.IsNullOrEmpty(_password);

        public string StartParameters()
        {
            return "-login " + AccountName + " " + Password;
        }

        public override string ToString()
        {
            return _name + "~ (user: " + AccountName + ")";
        }
    }
}