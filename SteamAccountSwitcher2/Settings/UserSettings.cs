using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher2.Settings
{
    class UserSettings
    {
        private string steamInstallDir;
        private string ecryption;
        private bool safemode;
        private bool autostart;

        public string SteamInstallDir
        {
            get => steamInstallDir;
            set => steamInstallDir = value;
        }

        public string Ecryption
        {
            get => ecryption;
            set => ecryption = value;
        }

        public bool Safemode
        {
            get => safemode;
            set => safemode = value;
        }

        public bool Autostart
        {
            get => autostart;
            set => autostart = value;
        }

        public bool Equals(UserSettings otherUserSettings)
        {
            return otherUserSettings.SteamInstallDir == this.SteamInstallDir &&
                   otherUserSettings.Ecryption == this.Ecryption &&
                   otherUserSettings.Safemode == this.Safemode &&
                   otherUserSettings.Autostart == this.Autostart;

        }
    }
}
