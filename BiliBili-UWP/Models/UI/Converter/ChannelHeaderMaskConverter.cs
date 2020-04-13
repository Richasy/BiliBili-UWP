using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Interface;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class ChannelHeaderMaskConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var data = value as IAlphaBackground;
            if (data != null)
            {
                var color = ColorHelper.ToColor(data.theme_color);
                return new SolidColorBrush(color) { Opacity = data.alpha / 100d };
            }
            return new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
