using System.Collections.Generic;
using System.IO;

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
        private bool _cachedAccount;

        private const string ImageFolder = "images";

        public SteamAccount()
        {
            // Empty constructor must exist for the JSON Converter!
        }

        public SteamAccount(string accountName, string password)
        {
            Name = accountName;
            AccountName = accountName;
            Password = password;
            Type = AccountType.Main;
            CachedAccount = false;
        }

        public SteamAccount(string steamId, string accountName, string personaName, bool rememberPassword,
            bool mostrecent, long timestamp)
        {
            Name = PersonaName = personaName;
            SteamId = steamId;
            AccountName = accountName;
            PersonaName = personaName;
            RememberPassword = rememberPassword;
            Mostrecent = mostrecent;
            Timestamp = timestamp;
            Type = AccountType.Main;
            CachedAccount = true;
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

        public string AccountImage => Path.Combine(SasManager.Instance.SteamInstallation.PathToSteamInstallationFolder, "config\\avatarcache", SteamId + ".png");

        public string BGImage => ImageFolder + "/acc-bg-" + _type.ToString().ToLower() + ".jpg";

        //public bool IsCached => string.IsNullOrEmpty(_password);
        public bool CachedAccount
        {
            get => _cachedAccount;
            set => _cachedAccount = value;
        }

        public string StartParameters()
        {
            return "-login " + AccountName + " " + Password;
        }

        public override string ToString()
        {
            return Name + " (Username: " + AccountName + ")";
        }

        protected bool Equals(SteamAccount other)
        {
            return Name == other._name && _accountName == other._accountName && _password == other._password && _type == other._type && _steamId == other._steamId && _personaName == other._personaName && _cachedAccount == other._cachedAccount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SteamAccount) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_accountName != null ? _accountName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_password != null ? _password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) _type;
                hashCode = (hashCode * 397) ^ (_steamId != null ? _steamId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_personaName != null ? _personaName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _cachedAccount.GetHashCode();
                return hashCode;
            }
        }
    }
}