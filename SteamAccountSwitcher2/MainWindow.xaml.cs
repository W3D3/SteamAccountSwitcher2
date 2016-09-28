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
        ObservableCollection<SteamAccount> accountList = new ObservableCollection<SteamAccount>();
        Steam steam;
        AccountLoader loader;
        bool autosaveAccounts = true;
        public MainWindow()
        {
            AutoUpdater.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); //Workaround for horrible AutoUpdater translations :D
            AutoUpdater.Start("http://wedenig.org/SteamAccountSwitcher/version.xml");

            InitializeComponent();
            //statusBarLabel.Content = AppDomain.CurrentDomain.BaseDirectory; //Debug location

            //No steam directory in Settings, let's find 'em!
            if (Properties.Settings.Default.steamInstallDir == String.Empty)
            {
                //Run this on first start
                string installDir = UserInteraction.selectSteamDirectory(@"C:\Program Files (x86)\Steam");
                if (installDir == null)
                {
                    MessageBox.Show("You cannot use SteamAccountSwitcher without selecting your Steam.exe. Program will close now.", "Steam missing", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                else
                {
                    Properties.Settings.Default.steamInstallDir = installDir;
                    steam = new Steam(installDir);
                }
        
            }
            else
            {
                //Start steam from existing installation path
                steam = new Steam(Properties.Settings.Default.steamInstallDir);
            }

            //statusBarLabel.Content = "Steam running in '" + Properties.Settings.Default.steamInstallDir + "'";
            statusBarLabel.Content = SteamStatus.steamStatusMessage();
            statusbar.Background = SteamStatus.getStatusColor();

            loader = new AccountLoader(Encryption.Basic);
            
            //accountList = new ObservableCollection<SteamAccount>(loader.LoadBasicAccounts());
            if (loader.AccountFileExists())
            {
                //Try to get accounts
                try
                { 
                    accountList = new ObservableCollection<SteamAccount>(loader.LoadBasicAccounts());
                }
                catch
                {
                    MessageBox.Show("Account file is currupted or wrong encryption method is set. Check Settings and try again. AutoSave has been disabled so that nothing can be overwritten! Make sure to restart the applications after switching Encryption method!", "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    accountList = new ObservableCollection<SteamAccount>();
                    autosaveAccounts = false;
                }
            }
            else
            {
                accountList = new ObservableCollection<SteamAccount>();
            }

            SteamAccount sa = new SteamAccount("username", "testpw");
            sa.Name = "profile name";
            //accountList.Add(sa);

            listBoxAccounts.ItemsSource = accountList;
            listBoxAccounts.Items.Refresh();

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //take full width
            itemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listBoxAccounts.ItemContainerStyle = itemContainerStyle;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //User has exited the application, save all data
            if (autosaveAccounts)
            {
                loader.SaveBasicAccounts(accountList.ToList<SteamAccount>());
            }
            Properties.Settings.Default.Save();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow newAccWindow = new AccountWindow();
            newAccWindow.Owner = this;
            newAccWindow.ShowDialog();
        }

        private void listBoxAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(sender.ToString());
            if (sender != null)
            {
                buttonEdit.IsEnabled = true;
            }
            listBoxAccounts.Items.Refresh();

        }

        private void listBoxAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SteamAccount selectedAcc = (SteamAccount)listBoxAccounts.SelectedItem;
            if (Properties.Settings.Default.safemode)
            {
                steam.StartSteamAccountSafe(selectedAcc);
            }
            else
            {
                steam.StartSteamAccount(selectedAcc);
            }
            
        }

        private void listContextMenuRemove_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(listBoxAccounts.SelectedItem.ToString());
            accountList.Remove((SteamAccount)listBoxAccounts.SelectedItem);
            listBoxAccounts.Items.Refresh();
        }

        private void listContextMenuEdit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
