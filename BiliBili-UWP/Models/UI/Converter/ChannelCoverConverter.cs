using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class ChannelCoverConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value==null || !value.ToString().StartsWith("http"))
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/captcha_refresh.png"));
            }
            return new BitmapImage(new Uri(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
