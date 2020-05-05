using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Video
{
    /// <summary>
    /// 字幕
    /// </summary>
    public class VideoSubtitleIndex
    {
        public bool allow_submit { get; set; }
        public List<SubtitleIndexItem> subtitles { get; set; }
    }
    public class SubtitleIndexItem
    {
        public long id { get; set; }
        public string lan { get; set; }
        public string lan_doc { get; set; }
        public string subtitle_url { get; set; }
        public static SubtitleIndexItem UnSelected = new SubtitleIndexItem
        {
            id = 0,
            lan = "",
            lan_doc = "无字幕",
            subtitle_url = ""
        };
    }
    public class VideoSubtitle
    {
        public double font_size { get; set; }
        public string font_color { get; set; }
        public double background_alpha { get; set; }
        public string background_color { get; set; }
        public string Stroke { get; set; }

        public List<SubtitleItem> body { get; set; }
    }
    public class SubtitleItem
    {
        public double from { get; set; }
        public double to { get; set; }
        public int location { get; set; }
        public string content { get; set; }
    }
}
