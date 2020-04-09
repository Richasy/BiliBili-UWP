using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliBili_UWP.Models.UI.Others
{
    public class TextBlockService: DependencyObject
    {
        public static readonly DependencyProperty IsAutoTrimProperty =
    DependencyProperty.RegisterAttached(
      "IsAutoTrim",
      typeof(bool),
      typeof(TextBlock),
      new PropertyMetadata(false,new PropertyChangedCallback(IsAutoTrim_Changed))
    );

        private static void IsAutoTrim_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var block = d as TextBlock;
            block.IsTextTrimmedChanged -= TrimChangedHandle;
            if ((bool)e.NewValue)
                block.IsTextTrimmedChanged += TrimChangedHandle;
        }

        public static void SetIsAutoTrim(UIElement element, bool value)
        {
            element.SetValue(IsAutoTrimProperty, value);
        }
        public static bool GetIsAutoTrim(UIElement element)
        {
            return (bool)element.GetValue(IsAutoTrimProperty);
        }

        public static void TrimChangedHandle(TextBlock sender, IsTextTrimmedChangedEventArgs args)
        {
            if (sender.IsTextTrimmed)
            {
                ToolTipService.SetToolTip(sender, sender.Text);
            }
            else
            {
                ToolTipService.SetToolTip(sender, null);
            }
        }
    }
}
