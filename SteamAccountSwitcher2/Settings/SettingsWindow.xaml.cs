using System;
using System.Windows;
using SteamAccountSwitcher2.Settings;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private UserSettings _windowSettings;

        public SettingsWindow()
        {
            InitializeComponent();
            _windowSettings = SasManager.Instance.UserSettings.Copy();

            textSteamInstallDir.Text = _windowSettings.SteamInstallDir;
            checkBoxAutostart.IsChecked = _windowSettings.Autostart;

            //Initialize Settings
            try
            {
                EncryptionType enc = (EncryptionType) Enum.Parse(typeof(EncryptionType), _windowSettings.EcryptionType);
                switch (enc)
                {
                    case EncryptionType.Basic:
                        radioButtonBasicEnc.IsChecked = true;
                        break;
                    case EncryptionType.Password:
                        radioButtonPasswordEnc.IsChecked = true;
                        break;
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(
                    "EncryptionType type not supported! Make sure you are using the latest SteamAccountSwitcher!",
                    "Unspported EncryptionType", MessageBoxButton.OK, MessageBoxImage.Error);
                radioButtonBasicEnc.IsChecked = false;
                radioButtonBasicEnc.IsEnabled = false;
                radioButtonPasswordEnc.IsChecked = false;
                radioButtonPasswordEnc.IsEnabled = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!SasManager.Instance.UserSettings.Equals(_windowSettings))
            {
                MessageBox.Show("Discard changes?");
            }
        }

        private void buttonBrowseSteamInstallDir_Click(object sender, RoutedEventArgs e)
        {
            string installDir = UserInteraction.selectSteamDirectory(@"C:\Program Files (x86)\Steam");
            if (installDir != null)
            {
                _windowSettings.SteamInstallDir = installDir;
                textSteamInstallDir.Text = _windowSettings.SteamInstallDir;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            _windowSettings.Autostart = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _windowSettings.Autostart = false;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.ApplyUserSettings(_windowSettings);
        }
    }
}