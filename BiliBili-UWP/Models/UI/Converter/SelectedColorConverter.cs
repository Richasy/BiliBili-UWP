using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BiliBili_UWP.Models.UI.Converter
{
    public class SelectedColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isSelect = false;
            if(value is int num)
                isSelect = num == 1;
            else
                isSelect = (bool)value;
            if(parameter==null)
                return isSelect ? UIHelper.GetThemeBrush(Enums.ColorType.SecondaryColor) : UIHelper.GetThemeBrush(Enums.ColorType.CardBackground);
            else if(parameter.ToString()=="Foreground")
                return isSelect ? new SolidColorBrush(Colors.White) : UIHelper.GetThemeBrush(Enums.ColorType.NormalTextColor);
            else
                return isSelect ? UIHelper.GetThemeBrush(Enums.ColorType.PrimaryColor) : UIHelper.GetThemeBrush(Enums.ColorType.NormalTextColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
