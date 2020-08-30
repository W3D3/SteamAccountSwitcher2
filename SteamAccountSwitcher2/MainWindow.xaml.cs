using AutoUpdaterDotNET;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AutoUpdater.Start("https://wedenig.org/SteamAccountSwitcher/version.xml");

            InitializeComponent();

            //Restore size
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            FixOutOfBoundsWindow();

            AskUserForSteamLocation();

            ShowSteamStatus();


            try
            {
                SasManager.Instance.InitializeAccountsFromFile();
            }
            catch
            {
                MessageBox.Show(
                    "Account file is currupted or wrong encryption method is set. Check Settings and try again. Save on close has been disabled so that nothing can be overwritten! Make sure to restart the applications after switching EncryptionType method!",
                    "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


            listBoxAccounts.ItemsSource = SasManager.Instance.AccountList;
            listBoxAccounts.Items.Refresh();

            var itemContainerStyle = new Style(typeof(ListBoxItem));
            //take full width
            itemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listBoxAccounts.ItemContainerStyle = itemContainerStyle;
        }

        private void AskUserForSteamLocation()
        {
            //No steam directory in Settings, let's find 'em!
            if (Properties.Settings.Default.steamInstallDir != string.Empty) 
                return;

            //Run this on first start
            var installDir = UserInteraction.SelectSteamDirectory(@"C:\Program Files (x86)\Steam");
            if (installDir == null)
            {
                MessageBox.Show(
                    "You cannot use SteamAccountSwitcher without selecting your Steam.exe. Program will close now.",
                    "Steam missing", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            else
            {
                SasManager.Instance.SetSteamInstallDir(installDir);
            }
        }

        private void ShowSteamStatus()
        {
            statusBarLabel.Content = SasManager.Instance.SteamStatus.SteamStatusMessage();
            statusbar.Background = SasManager.Instance.SteamStatus.GetStatusColor();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow {Owner = this};
            settingsWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SasManager.Instance.SaveAccounts();

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
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Height = Height;
                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var newAccWindow = new AccountWindow {Owner = this};
            newAccWindow.ShowDialog();
            if (newAccWindow.Account != null)
            {
                SasManager.Instance.AccountList.Add(newAccWindow.Account);
            }
        }

        private void ListBoxAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEdit.IsEnabled = sender != null;

            listBoxAccounts.Items.Refresh();
        }

        private void ListContextMenuRemove_Click(object sender, RoutedEventArgs e)
        {
            AskForDeletionOfAccount((SteamAccount) listBoxAccounts.SelectedItem);
        }

        private void ListContextMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAccounts.SelectedItem == null) 
                return;

            var newAccWindow = new AccountWindow((SteamAccount) listBoxAccounts.SelectedItem) {Owner = this};
            newAccWindow.ShowDialog();
            listBoxAccounts.Items.Refresh();
        }

        private void FixOutOfBoundsWindow()
        {
            var outOfBounds =
                (this.Left <= SystemParameters.VirtualScreenLeft - this.Width) ||
                (this.Top <= SystemParameters.VirtualScreenTop - this.Height) ||
                (SystemParameters.VirtualScreenLeft +
                 SystemParameters.VirtualScreenWidth <= this.Left) ||
                (SystemParameters.VirtualScreenTop +
                 SystemParameters.VirtualScreenHeight <= this.Top);

            if (!outOfBounds) 
                return;

            Debug.WriteLine("Out of bounds window was reset to default offsets");
            Left = 0;
            Top = 0;
            Width = 450;
            Height = 400;
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ListContextMenuEdit_Click(sender, e);
        }

        private void ButtonScanAccounts_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.ScanAndAddAccounts();
            listBoxAccounts.Items.Refresh();
        }

        private void SteamAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var selectedAcc = (SteamAccount) listBoxAccounts.SelectedItem;
                SasManager.Instance.StartSteamWithAccount(selectedAcc);
            }
        }

        /// <summary>
        /// Handles key downs on listBox with steam accounts
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                AskForDeletionOfAccount((SteamAccount) listBoxAccounts.SelectedItem);
            }
        }

        private void AskForDeletionOfAccount(SteamAccount selectedAccount)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete the account profile of " + selectedAccount.ToString() + "?",
                "Deletion prompt",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SasManager.Instance.AccountList.Remove(selectedAccount);
                buttonEdit.IsEnabled = false; // Cannot edit deleted account
                listBoxAccounts.Items.Refresh();
            }
        }
    }
}