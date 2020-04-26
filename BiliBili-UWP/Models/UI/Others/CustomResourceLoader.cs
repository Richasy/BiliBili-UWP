using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Resources;

namespace BiliBili_UWP.Models.UI.Others
{
    public class CustomResourceLoader: CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            if (resourceId == "Basic")
            {
                return new FontFamily(AppTool.GetLocalSetting(BiliBili_Lib.Enums.Settings.FontFamily,"微软雅黑"));
            }
            return null;
        }
    }
}
