using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoUpdaterDotNET;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //ObservableCollection<SteamAccount> accountList = new ObservableCollection<SteamAccount>();

        public MainWindow()
        {
            AutoUpdater.CurrentCulture =
                CultureInfo.CreateSpecificCulture("en-US"); //Workaround for horrible AutoUpdater translations :D
            AutoUpdater.Start("https://wedenig.org/SteamAccountSwitcher/version.xml");

            InitializeComponent();

            //Restore size
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            fixOutOfBoundsWindow();

            askUserForSteamLocation();

            showSteamStatus();


            try
            {
                SasManager.Instance.initializeAccountsFromFile();
            }
            catch
            {
                MessageBox.Show(
                    "Account file is currupted or wrong encryption method is set. Check Settings and try again. Save on close has been disabled so that nothing can be overwritten! Make sure to restart the applications after switching EncryptionType method!",
                    "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


            SteamAccount sa = new SteamAccount("username", "testpw");
            sa.Name = "profile name";
            //accountList.Add(sa);

            listBoxAccounts.ItemsSource = SasManager.Instance.AccountList;
            listBoxAccounts.Items.Refresh();

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //take full width
            itemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listBoxAccounts.ItemContainerStyle = itemContainerStyle;
        }

        private void askUserForSteamLocation()
        {
            //No steam directory in Settings, let's find 'em!
            if (Properties.Settings.Default.steamInstallDir == String.Empty)
            {
                //Run this on first start
                string installDir = UserInteraction.selectSteamDirectory(@"C:\Program Files (x86)\Steam");
                if (installDir == null)
                {
                    MessageBox.Show(
                        "You cannot use SteamAccountSwitcher without selecting your Steam.exe. Program will close now.",
                        "Steam missing", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                else
                {
                    SasManager.Instance.setSteamInstallDir(installDir);
                }
            }
        }

        private void showSteamStatus()
        {
            statusBarLabel.Content = SasManager.Instance.SteamStatus.steamStatusMessage();
            statusbar.Background = SasManager.Instance.SteamStatus.getStatusColor();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SasManager.Instance.saveOnExit();

            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow newAccWindow = new AccountWindow();
            newAccWindow.Owner = this;
            newAccWindow.ShowDialog();
            if (newAccWindow.Account != null)
            {
                SasManager.Instance.AccountList.Add(newAccWindow.Account);
            }
        }

        private void listBoxAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                buttonEdit.IsEnabled = true;
            }
            else
            {
                buttonEdit.IsEnabled = false;
            }

            listBoxAccounts.Items.Refresh();
        }

        private void listBoxAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SteamAccount selectedAcc = (SteamAccount) listBoxAccounts.SelectedItem;
            SasManager.Instance.startSteamWithAccount(selectedAcc);
        }

        private void listContextMenuRemove_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.AccountList.Remove((SteamAccount) listBoxAccounts.SelectedItem);
            buttonEdit.IsEnabled = false; // Cannot edit deleted account
            listBoxAccounts.Items.Refresh();
        }

        private void listContextMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAccounts.SelectedItem != null)
            {
                AccountWindow newAccWindow = new AccountWindow((SteamAccount) listBoxAccounts.SelectedItem);
                newAccWindow.Owner = this;
                newAccWindow.ShowDialog();
                if (newAccWindow.Account != null)
                {
                    SasManager.Instance.AccountList[listBoxAccounts.SelectedIndex] = newAccWindow.Account;
                    listBoxAccounts.SelectedItem = newAccWindow.Account;
                }
            }
        }

        private void fixOutOfBoundsWindow()
        {
            bool outOfBounds =
                (this.Left <= SystemParameters.VirtualScreenLeft - this.Width) ||
                (this.Top <= SystemParameters.VirtualScreenTop - this.Height) ||
                (SystemParameters.VirtualScreenLeft +
                 SystemParameters.VirtualScreenWidth <= this.Left) ||
                (SystemParameters.VirtualScreenTop +
                 SystemParameters.VirtualScreenHeight <= this.Top);

            if (outOfBounds)
            {
                Debug.WriteLine("Out of bounds window was reset to default offsets");
                this.Left = 0;
                this.Top = 0;
                this.Width = 450;
                this.Height = 400;
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            listContextMenuEdit_Click(sender, e);
        }
    }
}