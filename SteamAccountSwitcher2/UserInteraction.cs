using Microsoft.Win32;

namespace SteamAccountSwitcher2
{
    public class UserInteraction
    {
        public static string SelectSteamDirectory(string initialDirectory)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Steam |steam.exe",
                InitialDirectory = initialDirectory,
                Title = "Select your Steam Installation"
            };
            return (dialog.ShowDialog() == true) ? dialog.FileName : null;
        }
    }
}
