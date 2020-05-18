using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class StringVisbilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
            else
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
