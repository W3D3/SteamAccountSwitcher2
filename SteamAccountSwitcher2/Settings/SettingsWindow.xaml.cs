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

            RefreshGUIWithSettings();
        }

        private void RefreshGUIWithSettings()
        {
            //Initialize Settings to GUI
            _windowSettings = SasManager.Instance.GlobalSettings;

            textSteamInstallDir.Text = _windowSettings.SteamInstallDir;
            checkBoxAutostart.IsChecked = _windowSettings.Autostart;

            EncryptionType enc = _windowSettings.EcryptionType;
            switch (enc)
            {
                case EncryptionType.Basic:
                    radioButtonBasicEnc.IsChecked = true;
                    PasswordOptionsGroupBox.Visibility = Visibility.Collapsed;
                    break;
                case EncryptionType.Password:
                    radioButtonPasswordEnc.IsChecked = true;
                    PasswordOptionsGroupBox.Visibility = Visibility.Visible;
                    break;
                default:
                    MessageBox.Show(
                        "Encryption type not supported! Make sure you are using the latest SteamAccountSwitcher!",
                        "Unspported Encryption Type", MessageBoxButton.OK, MessageBoxImage.Error);
                    radioButtonBasicEnc.IsChecked = false;
                    radioButtonBasicEnc.IsEnabled = false;
                    radioButtonPasswordEnc.IsChecked = false;
                    radioButtonPasswordEnc.IsEnabled = false;
                    break;
            }
        }

        public bool HasUnwrittenChanges()
        {
            return !SasManager.Instance.GlobalSettings.Equals(_windowSettings);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var doClose = TryClosing();
            e.Cancel = !doClose;
        }

        private void buttonBrowseSteamInstallDir_Click(object sender, RoutedEventArgs e)
        {
            string installDir = UserInteraction.selectSteamDirectory(@"C:\Program Files (x86)\Steam");
            if (installDir != null)
            {
                _windowSettings.SteamInstallDir = installDir;
                textSteamInstallDir.Text = _windowSettings.SteamInstallDir;
                MadeChange();
            }
        }

        private void MadeChange()
        {
            // TODO refactor this using events or something
            buttonApply.IsEnabled = HasUnwrittenChanges();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            _windowSettings.Autostart = true;
            MadeChange();
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _windowSettings.Autostart = false;
            MadeChange();
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.ApplyUserSettings(_windowSettings);
            buttonApply.IsEnabled = HasUnwrittenChanges();
            RefreshGUIWithSettings();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.ApplyUserSettings(_windowSettings);
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private bool TryClosing()
        {
            if (!SasManager.Instance.GlobalSettings.Equals(_windowSettings))
            {
                var result = MessageBox.Show("Are you sure you want to discard changed settings?", "Unsaved changes",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                return result == MessageBoxResult.Yes;
            }

            return true;
        }

        private void radioButtonBasicEnc_Checked(object sender, RoutedEventArgs e)
        {
            _windowSettings.EcryptionType = EncryptionType.Basic;
            MadeChange();
        }

        private void radioButtonPasswordEnc_Checked(object sender, RoutedEventArgs e)
        {
            _windowSettings.EcryptionType = EncryptionType.Password;
            MadeChange();
        }

        private void ButtonSetPassword_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.SetEncryption(EncryptionType.Password);
        }
    }
}