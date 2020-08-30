using System;

namespace SteamAccountSwitcher2.Settings
{
    public class UserSettings
    {
        private bool _globalSettings;
        private string _steamInstallDir;
        private EncryptionType _encryptionType;
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
                _encryptionType =
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

        public EncryptionType EncryptionType
        {
            get => _encryptionType;
            set
            {
                _encryptionType = value;
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
                   _encryptionType == otherUserSettings._encryptionType &&
                   _autostart == otherUserSettings._autostart;
        }

        public UserSettings Copy()
        {
            var copySettings = new UserSettings
            {
                Autostart = Autostart,
                SteamInstallDir = SteamInstallDir,
                EncryptionType = EncryptionType
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
                hashCode = (hashCode * 397) ^ (_encryptionType != null ? _encryptionType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _autostart.GetHashCode();
                return hashCode;
            }
        }
    }
}