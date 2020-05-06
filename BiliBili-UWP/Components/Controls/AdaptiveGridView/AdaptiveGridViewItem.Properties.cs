using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BiliBili_UWP.Components.Controls.AdaptiveGridView
{
    public partial class AdaptiveGridViewItem
    {
        public static void SetIsTitle(DependencyObject element, bool value)
        {
            element.SetValue(IsTitleProperty, value);
        }
        public static bool GetIsTitle(DependencyObject element)
        {
            return (bool)element.GetValue(IsTitleProperty);
        }
        public static readonly DependencyProperty IsTitleProperty = DependencyProperty.RegisterAttached(
            "IsTitle", typeof(bool), typeof(AdaptiveGridViewItem), new PropertyMetadata(default(bool)));
    }
}
