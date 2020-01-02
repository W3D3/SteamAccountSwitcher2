using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Steam account without password
    /// </summary>
    class CachedAccount : SteamAccount
    {
        public CachedAccount(string steamId, string accountName, string personaName, bool rememberPassword, bool mostrecent, long timestamp)
        {
            SteamId = steamId;
            AccountName = accountName;
            Username = AccountName; // for UI
            PersonaName = personaName;
            Name = PersonaName; // for UI
            RememberPassword = rememberPassword;
            Mostrecent = mostrecent;
            Timestamp = timestamp;
        }

        public string SteamId { get; set; }

        public string AccountName { get; set; }

        public string PersonaName { get; set; }

        public bool RememberPassword { get; set; }

        public bool Mostrecent { get; set; }

        public long Timestamp { get; set; }

        public bool WantsOfflineMode { get; set; }
    }
}