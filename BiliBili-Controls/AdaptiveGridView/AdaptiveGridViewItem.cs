using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BiliBili_Controls.AdaptiveGridView
{
    public partial class AdaptiveGridViewItem : GridViewItem
    {
        public AdaptiveGridViewItem()
        {
            DefaultStyleKey = typeof(AdaptiveGridViewItem);

            PointerEntered += OnPointerEntered;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var template = ContentTemplate;
            if (template.LoadContent() is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (GetIsTitle(child))
                    {
                        var title = child;
                    }
                }
            }
        }
    }
}
