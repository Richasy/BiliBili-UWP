using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliBili_UWP.Models.UI.Others
{
    public class SideMenuItemTemplateSelector: DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate LineTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object data)
        {
            var item = data as SideMenuItem;
            return item.Type == Enums.SideMenuItemType.Line ? LineTemplate : DefaultTemplate;
        }
    }
}
