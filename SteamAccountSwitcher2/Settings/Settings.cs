using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher2.Settings
{
    /// <summary>
    /// Contains the actual settings for this SteamAccountSwitcher
    /// </summary>
    class Settings
    {
        // Properties.Settings.Default.steamInstallDir;
        private string _steamInstallDir;
        private EncryptionType _encryptionType;
        private bool _safeModeEnabled;

        public Settings()
        {
            _steamInstallDir = Properties.Settings.Default.steamInstallDir;
        }
    }
}