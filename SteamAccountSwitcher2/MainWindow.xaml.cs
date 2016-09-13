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

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        ObservableCollection<SteamAccount> accountList = new ObservableCollection<SteamAccount>();
        public MainWindow()
        {
            InitializeComponent();
            statusBarLabel.Content = AppDomain.CurrentDomain.BaseDirectory;

            //sample data
            SteamAccount sa = new SteamAccount("W3D3", "test");
            sa.Name = "new acc";
            accountList.Add(sa);

            SteamAccount sa1 = new SteamAccount("Tst", "test");
            sa1.Name = "nsadc";
            accountList.Add(sa1);

            listBoxAccounts.ItemsSource = accountList;
            listBoxAccounts.Items.Refresh();

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //take full width
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));

            //drag and drop logic
            /*itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseLeftButtonDown)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));*/
            listBoxAccounts.ItemContainerStyle = itemContainerStyle;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow();
            Properties.Settings.Default.steamInstallDir = "abc";
            window.ShowDialog();
            statusBarLabel.Content = Properties.Settings.Default.steamInstallDir;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //User has exited the application, save all data
            Properties.Settings.Default.Save();
        }
    }
}
