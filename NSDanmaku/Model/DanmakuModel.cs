using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace NSDanmaku.Model
{
    public enum DanmakuLocation
    {
        /// <summary>
        /// 滚动弹幕Model1-3
        /// </summary>
        Roll,
        /// <summary>
        /// 顶部弹幕Model5
        /// </summary>
        Top,
        /// <summary>
        /// 底部弹幕Model4
        /// </summary>
        Bottom,
        /// <summary>
        /// 定位弹幕Model7
        /// </summary>
        Position,
        /// <summary>
        /// 其它暂未支持的类型
        /// </summary>
        Other
    }
    public enum DanmakuSite
    {
        Bilibili,
        Acfun,
        Tantan
    }
    public enum DanmakuBorderStyle
    {
        Default=0,
        NoBorder=1,
        Shadow=2,
        BorderV2=3
    }
    public enum DanmakuMode
    {
        Video,
        Live
    }
    public class DanmakuModel
    {
        public string text { get; set; }
        /// <summary>
        /// 弹幕大小
        /// </summary>
        public double size { get; set; }
        /// <summary>
        /// 弹幕颜色
        /// </summary>
        public Color color { get; set; }
        /// <summary>
        /// 弹幕出现时间
        /// </summary>
        public double time { get; set; }

        /// <summary>
        /// 弹幕出现时间
        /// </summary>
        public int time_s{ get; set; }
        /// <summary>
        /// 弹幕发送时间
        /// </summary>
        public string sendTime { get; set; }
        /// <summary>
        /// 弹幕池
        /// </summary>
        public string pool { get; set; }
        /// <summary>
        /// 弹幕发送人ID
        /// </summary>
        public string sendID { get; set; }
        /// <summary>
        /// 弹幕ID
        /// </summary>
        public string rowID { get; set; }
        /// <summary>
        /// 弹幕出现位置
        /// </summary>
        public DanmakuLocation location
        {
            get; set;
        }

        public DanmakuSite fromSite { get; set; }

        public string source { get; set; }


        public SolidColorBrush colorBrush
        {
            get
            {
                return new SolidColorBrush(color);
            }
        }
    }
}
