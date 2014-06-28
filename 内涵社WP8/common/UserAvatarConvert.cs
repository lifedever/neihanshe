using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using neihanshe.Resources;

namespace neihanshe.common
{
    public class UserAvatarConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string avatarUri = null;
            ImageSource imageSource = null;
            if (value != null)
            {
                avatarUri = string.Format("http://avatar.neihanshe.cn/avatar/{0}.jpg", value);
                imageSource = new BitmapImage(new Uri(avatarUri, UriKind.Absolute));
            }
            else
            {
                imageSource = ByteArrayToImage(AppResources.headshot);
            }

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public static BitmapImage ByteArrayToImage(byte[] bits)
        {
            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream(bits))
            {
                bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
                bitmapImage.SetSource(ms);
                return bitmapImage;
            }
        }
    }
}