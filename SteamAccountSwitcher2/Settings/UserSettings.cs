using System;

namespace SteamAccountSwitcher2.Settings
{
    public class UserSettings
    {
        private bool _globalSettings;

        private string _steamInstallDir;
        private EncryptionType _ecryptionType;
        private bool _autostart;


        public UserSettings() : this(false)
        {

        }

        public UserSettings(bool global)
        {
            _globalSettings = global;
            if (_globalSettings)
            {
                _autostart = Properties.Settings.Default.autostart;
                _steamInstallDir = Properties.Settings.Default.steamInstallDir;
                _ecryptionType =
                    (EncryptionType) Enum.Parse(typeof(EncryptionType), Properties.Settings.Default.encryption);
            }
        }

        public string SteamInstallDir
        {
            get => _steamInstallDir;
            set
            {
                _steamInstallDir = value; 
                if(_globalSettings) Properties.Settings.Default.steamInstallDir = value; 
            }
        }

        public EncryptionType EcryptionType
        {
            get => _ecryptionType;
            set
            {
                _ecryptionType = value;
                if (_globalSettings) 
                    Properties.Settings.Default.encryption = value.ToString();
            }
        }

        public bool Autostart
        {
            get => _autostart;
            set
            {
                _autostart = value;
                if (_globalSettings) Properties.Settings.Default.autostart = value;
            }
        }

        protected bool Equals(UserSettings otherUserSettings)
        {
            return _steamInstallDir == otherUserSettings._steamInstallDir &&
                   _ecryptionType == otherUserSettings._ecryptionType &&
                   _autostart == otherUserSettings._autostart;
        }

        public UserSettings Copy()
        {
            var copySettings = new UserSettings
            {
                Autostart = Autostart,
                SteamInstallDir = SteamInstallDir,
                EcryptionType = EcryptionType
            };
            return copySettings;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_steamInstallDir != null ? _steamInstallDir.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_ecryptionType != null ? _ecryptionType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _autostart.GetHashCode();
                return hashCode;
            }
        }
    }
}