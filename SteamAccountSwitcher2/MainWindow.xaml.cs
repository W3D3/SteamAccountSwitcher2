using AutoUpdaterDotNET;
using Serilog;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
            // URL is not working
            //AutoUpdater.Start("https://wedenig.org/SteamAccountSwitcher/version.xml");

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException += Dispatcher_UnhandledException;

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
                SasManager.Instance.InitializeAccountsFromFile();
            }
            catch
            {
                MessageBox.Show(
                    "Account file is currupted or wrong encryption method is set. Check Settings and try again. Save on close has been disabled so that nothing can be overwritten! Make sure to restart the applications after switching EncryptionType method!",
                    "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


            //SteamAccount sa = new SteamAccount("username", "testpw");
            //sa.Name = "profile name";
            //accountList.Add(sa);

            listBoxAccounts.ItemsSource = SasManager.Instance.AccountList;
            listBoxAccounts.Items.Refresh();

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //take full width
            itemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listBoxAccounts.ItemContainerStyle = itemContainerStyle;
        }

    
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.Error(e.Exception, "Dispatcher_UnhandledException");
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
                    Properties.Settings.Default.steamInstallDir = installDir;
                    SasManager.InitiateInstanceWithDir(installDir);
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
            try
            {
                SasManager.Instance.SaveAccounts();

                if (this.WindowState == WindowState.Maximized)
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
            
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while saving accounts");
            }
            finally
            {
                Properties.Settings.Default.Save();
            }

            
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

        private void listContextMenuRemove_Click(object sender, RoutedEventArgs e)
        {
            AskForDeletionOfAccount((SteamAccount) listBoxAccounts.SelectedItem);
        }

        private void listContextMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAccounts.SelectedItem != null)
            {
                //MessageBox.Show(sender.ToString());
                AccountWindow newAccWindow = new AccountWindow((SteamAccount) listBoxAccounts.SelectedItem);
                newAccWindow.Owner = this;
                newAccWindow.ShowDialog();
                listBoxAccounts.Items.Refresh();
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

        private void buttonScanAccounts_Click(object sender, RoutedEventArgs e)
        {
            SasManager.Instance.ScanAndAddAccounts();
            listBoxAccounts.Items.Refresh();
        }

        private void steamAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                //MessageBox.Show(listBoxAccounts.SelectedItem.ToString());
                SteamAccount selectedAcc = (SteamAccount) listBoxAccounts.SelectedItem;
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