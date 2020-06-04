using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BiliBili_Controls.Extensions
{
    public class StackLayoutContext
    {
        public StackLayoutContext()
        {

        }

        public List<UIElement> Elements { get; private set; } = new List<UIElement>();

        public double Lenght { get; set; } = 0.0;

        public double Width { get; set; } = 0.0;

        public double Offset { get; set; } = 0.0;

        public int FirstRealizedItemIndex { get; set; } = -1;

        public int RealizedItemCount { get; set; } = 0;
    }
}
