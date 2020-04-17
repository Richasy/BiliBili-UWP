using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Video
{
    public class VideoDetail
    {
        public string bvid { get; set; }
        public int aid { get; set; }
        public int videos { get; set; }
        public int tid { get; set; }
        public string tname { get; set; }
        public int copyright { get; set; }
        public string pic { get; set; }
        public string title { get; set; }
        public int pubdate { get; set; }
        public int ctime { get; set; }
        public string desc { get; set; }
        public int state { get; set; }
        public int attribute { get; set; }
        public int duration { get; set; }
        public VideoOwner owner { get; set; }
        public VideoStat stat { get; set; }
        public string dynamic { get; set; }
        public int cid { get; set; }
        public bool no_cache { get; set; }
        public List<VideoPart> pages { get; set; }
        public VideoSubtitle subtitle { get; set; }
    }
    public class VideoOwner
    {
        public int mid { get; set; }
        public string name { get; set; }
        public string face { get; set; }
    }

    public class VideoStat
    {
        public int aid { get; set; }
        public int view { get; set; }
        public int danmaku { get; set; }
        public int reply { get; set; }
        public int favorite { get; set; }
        public int coin { get; set; }
        public int share { get; set; }
        public int now_rank { get; set; }
        public int his_rank { get; set; }
        public int like { get; set; }
        public int dislike { get; set; }
        public string evaluation { get; set; }
    }
    /// <summary>
    /// 字幕
    /// </summary>
    public class VideoSubtitle
    {
        public bool allow_submit { get; set; }
        public List<SubtitleItem> list { get; set; }
    }
    public class SubtitleItem
    {
        public long id { get; set; }
        public string lan { get; set; }
        public string lan_doc { get; set; }
        public string subtitle_url { get; set; }
    }
}
