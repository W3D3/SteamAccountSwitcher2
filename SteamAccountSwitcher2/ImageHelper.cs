using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamAccountSwitcher2
{
    public class ImageHelper
    {
        public static ImageSource GetIconImageSource(string name)
        {
            return new BitmapImage(new Uri(@"images/icons/" + name + ".png", UriKind.Relative));
        }
    }
}