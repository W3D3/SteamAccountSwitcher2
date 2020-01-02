using System;
using System.Windows;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private SteamAccount newAcc;

        /// <summary>
        /// Creates a new instance of the AccountWindow class. Allows the user to create new accounts.
        /// </summary>
        public AccountWindow()
        {
            InitializeComponent();
            comboBoxType.ItemsSource = Enum.GetValues(typeof(AccountType));
            comboBoxType.SelectedIndex = 0;
            labelIsCached.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Creates a new instance of the AccountWindow class. Allows the user to edit new accounts
        /// </summary>
        /// <param name="accToEdit">The account to edit.</param>
        public AccountWindow(SteamAccount accToEdit)
        {
            if (accToEdit == null)
                throw new ArgumentNullException();

            InitializeComponent();
            this.Title = "Edit Account";
            newAcc = accToEdit;

            comboBoxType.ItemsSource = Enum.GetValues(typeof(AccountType));
            comboBoxType.SelectedItem = accToEdit.Type;

            textBoxName.Text = accToEdit.Name;
            textBoxUsername.Text = accToEdit.AccountName;
            textBoxPassword.Password = accToEdit.Password;

            textBoxUsername.IsEnabled = accToEdit.CachedAccount;
            labelIsCached.Visibility = accToEdit.CachedAccount ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Accessor to the Account associated with the window.
        /// </summary>
        public SteamAccount Account => newAcc;

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                if (newAcc == null)
                {
                    newAcc = new SteamAccount(textBoxUsername.Text, textBoxPassword.Password);
                    newAcc.Type = (AccountType)comboBoxType.SelectedValue;
                    newAcc.Name = textBoxName.Text;
                }
                else
                {
                    newAcc.AccountName = textBoxUsername.Text;
                    newAcc.Password = textBoxPassword.Password;
                    newAcc.Name = textBoxName.Text;
                    newAcc.Type = (AccountType)comboBoxType.SelectedValue;
                }
                

                Close();
            }
        }

        private bool ValidateInput()
        {
            bool success = true;
            string errorstring = "";
            if (String.IsNullOrEmpty(textBoxName.Text))
            {
                success = false;
                errorstring += "Profile name cannot be empty!\n";
            }

            if (String.IsNullOrEmpty(textBoxUsername.Text))
            {
                success = false;
                errorstring += "Username cannot be empty!\n";
            }

            if (String.IsNullOrEmpty(textBoxPassword.Password) && labelIsCached.Visibility != Visibility.Visible)
            {
                success = false;
                errorstring += "Password cannot be empty!\n";
            }

            if (success)
            {
                return true;
            }
            else
            {
                MessageBox.Show(errorstring, "Validation problem", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            newAcc = null;
            Close();
        }
    }
}