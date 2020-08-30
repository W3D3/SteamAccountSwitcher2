using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Windows.Media;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// The <see cref="SteamStatus"/> Class is offering steam status information.
    /// </summary>
    public class SteamStatus
    {
        const string STATUS_API = "https://crowbar.steamstat.us/gravity.json";
        private bool _onlineStatusGood = false;

        public SteamStatus()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            _onlineStatusGood = CheckSteamStatus();
        }

        /// <summary>
        /// Checks Steam status by calling an external service.
        /// </summary>
        /// <returns>true if steam is up, false if not.</returns>
        private static bool CheckSteamStatus()
        {
            try
            {
                var client = new WebClient() { Encoding = Encoding.UTF8 };
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                var statusJson = client.DownloadString(STATUS_API);
                var status = JObject.Parse(statusJson);

                // TODO get a proper steam status check, this is unreliable
                var state = status["services"][2][1].ToString();
                if (state == "0")
                    return true;

                return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Generates a GUI friendly string describing Steam's status at the time of last refresh.
        /// </summary>
        /// <returns>GUI friendly <see cref="string"/>.</returns>
        public string SteamStatusMessage()
        {
            return _onlineStatusGood ? "Steam is operating normally." : "Steam is currently having issues!";
        }

        /// <summary>
        /// Generates a <see cref="SolidColorBrush"/> indicating Steam's status at the time of last refresh.
        /// </summary>
        /// <returns><see cref="SolidColorBrush"/> status indicator</returns>
        public SolidColorBrush GetStatusColor()
        {
            if (_onlineStatusGood)
            {
                var green = Color.FromRgb(146, 247, 181);
                var greenbrush = new SolidColorBrush(green);
                return greenbrush;
            }
            else
            {
                var red = Color.FromRgb(250, 165, 165);
                var redbrush = new SolidColorBrush(red);
                return redbrush;
            }
        }
    }
}
