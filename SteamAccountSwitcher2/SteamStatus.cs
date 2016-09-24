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
    class SteamStatus
    {
        const string STATUS_API = "https://steamgaug.es/api/v2";

        private static bool isSteamUp()
        {
            string statusJson = new WebClient().DownloadString(STATUS_API);
            JObject status = JObject.Parse(statusJson);

            string state = status["ISteamClient"]["online"].ToString();
            if(state == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
