using System.Windows;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private readonly bool _setNewPw;

        /// <summary>
        /// A password window where the user enters a password (duh?)
        /// </summary>
        /// <param name="setNewPw">Set to true to make the user enter a password twice for verification</param>
        public PasswordWindow(bool setNewPw)
        {
            _setNewPw = setNewPw;
            InitializeComponent();
            passwordBox.Focus();
            if (_setNewPw)
            {
                PwWindow.Title = "Set new password";
                PwWindow.Height = 140;
                repeatPasswordPanel.Visibility = Visibility.Visible;
                Image.Source = ImageHelper.GetIconImageSource("key");
            }
            else
            {
                PwWindow.Title = "Decrypt accounts with password";
                PwWindow.Height = 120;
                repeatPasswordPanel.Visibility = Visibility.Collapsed;
                Image.Source = ImageHelper.GetIconImageSource("unlock");
            }
        }

        public string Password { get; private set; }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == passwordBoxRepeat.Password || !_setNewPw)
            {
                Password = passwordBox.Password;
                Close();
            }
            else
            {
                MessageBox.Show("Passwords do not match. Try again.", "Passwords not matching", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}