using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows.Media;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// The <see cref="SteamStatus"/> Class is offering steam status information.
    /// </summary>
    public class SteamStatus
    {
        const string STATUS_API = "https://crowbar.steamstat.us/Barney";
        private bool onlineStatusGood = false;

        public SteamStatus()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            refreshStatus();
        }

        public void refreshStatus()
        {
            onlineStatusGood = true;// checkSteamStatus();
        }

        /// <summary>
        /// Checks Steam status by calling an external service.
        /// </summary>
        /// <returns>true if steam is up, false if not.</returns>
        private static bool checkSteamStatus()
        {
            try
            {
                string statusJson = new WebClient().DownloadString(STATUS_API);
                JObject status = JObject.Parse(statusJson);

                string state = status["services"]["online"]["status"].ToString();
                if (state == "good")
                    return true;
                else
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
        public string steamStatusMessage()
        {
            return onlineStatusGood ? "Steam is operating normally." : "Steam is currently having issues!";
        }

        /// <summary>
        /// Generates a <see cref="SolidColorBrush"/> indicating Steam's status at the time of last refresh.
        /// </summary>
        /// <returns><see cref="SolidColorBrush"/> status indicator</returns>
        public SolidColorBrush getStatusColor()
        {
            if (onlineStatusGood)
            {
                Color green = Color.FromRgb(146, 247, 181);
                SolidColorBrush greenbrush = new SolidColorBrush(green);
                return greenbrush;
            }
            else
            {
                Color red = Color.FromRgb(250, 165, 165);
                SolidColorBrush redbrush = new SolidColorBrush(red);
                return redbrush;
            }
        }
    }
}
