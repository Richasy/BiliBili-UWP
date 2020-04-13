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
                return new FontFamily("Taipei Sans TC Beta Light");
            }
            return null;
        }
    }
}
