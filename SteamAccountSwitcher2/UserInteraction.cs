using Microsoft.Win32;

namespace SteamAccountSwitcher2
{
    class UserInteraction
    {
        public static string selectSteamDirectory(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Steam |steam.exe";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select your Steam Installation";
            return (dialog.ShowDialog() == true)
               ? dialog.FileName : null;
        }
    }
}
