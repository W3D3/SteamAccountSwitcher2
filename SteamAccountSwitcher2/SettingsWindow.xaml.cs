using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Steam changedSteam;
        public Steam Steam
        {
            get { return changedSteam; }
            set { this.changedSteam = value; }
        }

        public SettingsWindow()
        {
            InitializeComponent();

            textSteamInstallDir.Text = Properties.Settings.Default.steamInstallDir;

            //Initialize Settings
            try
            {
                Encryption enc = (Encryption)Enum.Parse(typeof(Encryption), Properties.Settings.Default.encryption);
                if (enc == Encryption.Basic)
                {
                    radioButtonBasicEnc.IsChecked = true;
                }
                if (enc == Encryption.Password)
                {
                    radioButtonPasswordEnc.IsChecked = true;
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Encryption type not supported! Make sure you are using the latest SteamAccountSwitcher!","Unspported Encryption", MessageBoxButton.OK, MessageBoxImage.Error);
                radioButtonBasicEnc.IsChecked = false;
                radioButtonBasicEnc.IsEnabled = false;
                radioButtonPasswordEnc.IsChecked = false;
                radioButtonPasswordEnc.IsEnabled = false;
            }

            bool safemode = Properties.Settings.Default.safemode;
            safeModeToggle.IsChecked = safemode;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.safemode = safeModeToggle.IsChecked.Value;
        }

        private void buttonBrowseSteamInstallDir_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Properties.Settings.Default.steamInstallDir ?? @"C:\Program Files (x86)\Steam";
            string installDir = UserInteraction.selectSteamDirectory(initialDir);
            if (installDir != null)
            {
                Properties.Settings.Default.steamInstallDir = installDir;
                changedSteam = new Steam(installDir);
                textSteamInstallDir.Text = Properties.Settings.Default.steamInstallDir;
            }
        }
    }
}
