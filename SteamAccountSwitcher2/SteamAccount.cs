using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher2
{
    public class SteamAccount
    {
        string name;
        string username;
        string password;
        AccountType type;

        public SteamAccount()
        {

        }

        public SteamAccount(string username, string password)
        {
            this.name = username;
            this.username = username;
            this.password = password;
            this.type = AccountType.Main;
        }

        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        public string Username
        {
            get { return username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return password; }
            set { this.password = value; }
        }

        public AccountType Type
        {
            get { return type; }
            set { this.type = value; }
        }

        public string Icon
        {
            get
            {
                if (this.type == AccountType.Main)
                {
                    return "steam-ico-main.png";
                }
                if (this.type == AccountType.Smurf)
                {
                    return "steam-ico-smurf.png";
                }
                return null;
            }
        }

        public string Background
        {
            get
            {
                if (this.type == AccountType.Main)
                {
                    return "sas_acc_bg_50.jpg";
                }
                else if (this.type == AccountType.Smurf)
                {
                    return "sas_acc_bg_smurfy.jpg";
                }
                else
                {
                    return "sas_acc_bg_50.jpg";
                }
            }
        }

        public string BackgroundPath
        {
            get
            {
                return "images/" + this.Background;
            }
        }

        public string getStartParameters()
        {
            return "-login " + this.username + " " + this.password;
        }
        public override string ToString()
        {
            return name + "~ (user: " + username + ")";
        }
    }
}
