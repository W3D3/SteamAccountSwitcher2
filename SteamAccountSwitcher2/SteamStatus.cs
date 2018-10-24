using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// The <see cref="SteamStatus"/> Class is a static class offering steam status information.
    /// </summary>
    static class SteamStatus
    {
        const string STATUS_API = "https://crowbar.steamstat.us/Barney";

        /// <summary>
        /// Checks Steam status by calling an external service.
        /// </summary>
        /// <returns>true if steam is up, false if not.</returns>
        private static bool isSteamUp()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string statusJson = new WebClient().DownloadString(STATUS_API);
                JObject status = JObject.Parse(statusJson);

                string state = status["services"]["steam"]["status"].ToString();
                if (state == "good")
                    return true;
                else
                    return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

        }

        /// <summary>
        /// Generates a GUI friendly string describing Steam's current status.
        /// </summary>
        /// <returns>GUI friendly <see cref="string"/>.</returns>
        public static string steamStatusMessage()
        {
            if(isSteamUp())
            {
                return "Steam is operating normally.";
            }
            else
            {
                return "Steam is currently having issues!";
            }
        }

        /// <summary>
        /// Generates a <see cref="SolidColorBrush"/> indicating Steam's current status.
        /// </summary>
        /// <returns><see cref="SolidColorBrush"/> status indicator</returns>
        public static SolidColorBrush getStatusColor()
        {

            if (isSteamUp())
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
