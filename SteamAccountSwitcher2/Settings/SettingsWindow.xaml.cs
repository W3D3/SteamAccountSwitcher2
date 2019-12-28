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
        private Properties.Settings settings;

        public SettingsWindow()
        {
            InitializeComponent();
            settings = SasManager.Instance.Settings;

            textSteamInstallDir.Text = settings.steamInstallDir;

            //Initialize Settings
            try
            {
                EncryptionType enc = (EncryptionType)Enum.Parse(typeof(EncryptionType), Properties.Settings.Default.encryption);
                if (enc == EncryptionType.Basic)
                {
                    radioButtonBasicEnc.IsChecked = true;
                }
                if (enc == EncryptionType.Password)
                {
                    radioButtonPasswordEnc.IsChecked = true;
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("EncryptionType type not supported! Make sure you are using the latest SteamAccountSwitcher!","Unspported EncryptionType", MessageBoxButton.OK, MessageBoxImage.Error);
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
            string installDir = UserInteraction.selectSteamDirectory(@"C:\Program Files (x86)\Steam");
            if (installDir != null)
            {
                SasManager.Instance.setSteamInstallDir(installDir);
                textSteamInstallDir.Text = settings.steamInstallDir;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.setAutoStart(true);
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.setAutoStart(false);
        }
    }
}
