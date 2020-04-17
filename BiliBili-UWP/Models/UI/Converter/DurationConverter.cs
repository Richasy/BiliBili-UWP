using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class DurationConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int du = System.Convert.ToInt32(value);
            return AppTool.GetReadDuration(du);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
