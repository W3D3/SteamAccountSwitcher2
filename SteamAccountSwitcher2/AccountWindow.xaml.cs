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

            comboBoxType.ItemsSource = Enum.GetValues(typeof(AccountType));
            comboBoxType.SelectedItem = accToEdit.Type;

            textBoxName.Text  = accToEdit.Name;
            textBoxUsername.Text = accToEdit.AccountName;
            textBoxPassword.Password = accToEdit.Password;
        }

        /// <summary>
        /// Accessor to the Account associated with the window.
        /// </summary>
        public SteamAccount Account
        {
            get { return newAcc; }
        }

        
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                newAcc = new SteamAccount();
                newAcc.Type = (AccountType)comboBoxType.SelectedValue;
                newAcc.Name = textBoxName.Text;
                newAcc.AccountName = textBoxUsername.Text;
                newAcc.Password = textBoxPassword.Password;

                Close();
            }
        }

        private bool ValidateInput()
        {
            bool success = true;
            string errorstring = "";
            if(String.IsNullOrEmpty(textBoxName.Text))
            {
                success = false;
                errorstring += "Profile name cannot be empty!\n";
            }
            if (String.IsNullOrEmpty(textBoxUsername.Text))
            {
                success = false;
                errorstring += "Username cannot be empty!\n";
            }
            if (String.IsNullOrEmpty(textBoxPassword.Password))
            {
                success = false;
                errorstring += "Password cannot be empty!\n";
            }

            if(success)
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
