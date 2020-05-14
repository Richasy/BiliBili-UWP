using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class CoverResolutionConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value!=null && value is string url && !string.IsNullOrEmpty(url))
            {
                //string ext = Path.GetExtension(url);
                string suffix = "";
                if (parameter != null && !url.EndsWith("noface.gif"))
                    suffix = $"@{parameter.ToString()}w.jpg";
                return url + suffix;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
