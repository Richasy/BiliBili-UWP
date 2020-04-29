using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using h=Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Media;

namespace BiliBili_UWP.Models.UI.Others
{
    public class DanmakuColor
    {
        public string Name { get; set; }
        public SolidColorBrush Color { get; set; }
        public DanmakuColor()
        {

        }
        public DanmakuColor(string n,Color c)
        {
            Name = n;
            Color = new SolidColorBrush(c);
        }
        public static List<DanmakuColor> GetColorList()
        {
            //红*256^2+绿*256^1+蓝*256^0
            return new List<DanmakuColor>
            {
                new DanmakuColor("白色",h.ColorHelper.ToColor("#89D5FF")),
                new DanmakuColor("红色",h.ColorHelper.ToColor("#FE0302")),
                new DanmakuColor("橙黄色",h.ColorHelper.ToColor("#FFAA02")),
                new DanmakuColor("土黄色",h.ColorHelper.ToColor("#FFD302")),
                new DanmakuColor("黄色",h.ColorHelper.ToColor("#FFFF00")),
                new DanmakuColor("草绿色",h.ColorHelper.ToColor("#A0EE00")),
                new DanmakuColor("绿色",h.ColorHelper.ToColor("#00CD00")),
                new DanmakuColor("青色",h.ColorHelper.ToColor("#019899")),
                new DanmakuColor("紫色",h.ColorHelper.ToColor("#4266BE")),
                new DanmakuColor("浅蓝色",h.ColorHelper.ToColor("#89D5FF"))
            };
        }
    }
}
