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
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private string _password;
        private bool _repeatPassword;

        public PasswordWindow(bool repeatPassword)
        {
            _repeatPassword = repeatPassword;
            InitializeComponent();
            passwordBox.Focus();
            if (_repeatPassword)
            {
                PwWindow.Height = 140;
                repeatPasswordPanel.Visibility = Visibility.Visible;
            }
            else
            {
                PwWindow.Height = 120;
                repeatPasswordPanel.Visibility = Visibility.Collapsed;
            }
        }

        public string Password => _password;

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == passwordBoxRepeat.Password || !_repeatPassword)
            {
                _password = passwordBox.Password;
                Close();
            }
            else
            {
                MessageBox.Show("Passwords do not match. Try again.", "Passwords not matching", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
