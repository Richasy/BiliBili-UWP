using BiliBili_Lib.Models.BiliBili.Anime;
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
        public long tid { get; set; }
        public string tname { get; set; }
        public int copyright { get; set; }
        public string pic { get; set; }
        public string title { get; set; }
        public int pubdate { get; set; }
        public long ctime { get; set; }
        public string desc { get; set; }
        public int state { get; set; }
        public int attribute { get; set; }
        public ReqestUser req_user { get; set; }
        public int duration { get; set; }
        public VideoOwner owner { get; set; }
        public VideoStat stat { get; set; }
        public string dynamic { get; set; }
        public int cid { get; set; }
        public bool no_cache { get; set; }
        public List<VideoPart> pages { get; set; }
        public VideoSubtitle subtitle { get; set; }
        public List<VideoTag> tag { get; set; }
        public List<Staff> staff { get; set; }
        public List<VideoRelated> relates { get; set; }
        public int id { get; set; }
        public InteractionPart interaction { get; set; }
        public BangumiSlim bangumi { get; set; }
        public string redirect_url { get; set; }
        public History history { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VideoDetail detail &&
                   aid == detail.aid;
        }

        public override int GetHashCode()
        {
            return 1525766729 + aid.GetHashCode();
        }

        public class History
        {
            public int cid { get; set; }
            public int progress { get; set; }
        }

    }
    public class ReqestUser
    {
        public int attention { get; set; }
        public int favorite { get; set; }
        public int like { get; set; }
        public int dislike { get; set; }
        public int coin { get; set; }
    }
    public class Staff : Author
    {
        public string title { get; set; }
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
    
    public class VideoTag
    {
        public int tag_id { get; set; }
        public string tag_name { get; set; }
        public string cover { get; set; }
        public int likes { get; set; }
        public int hates { get; set; }
        public int liked { get; set; }
        public int hated { get; set; }
        public int attribute { get; set; }
        public int is_activity { get; set; }
        public string uri { get; set; }
        public string tag_type { get; set; }
    }

    public class VideoRelated
    {
        public int aid { get; set; }
        public string pic { get; set; }
        public string title { get; set; }
        public int cid { get; set; }
        public int duration { get; set; }
        public VideoOwner owner { get; set; }
        public VideoStat stat { get; set; }
        public string @goto { get; set; }
        public string param { get; set; }
        public string trackid { get; set; }
    }

    public class InteractionPart
    {
        public HistoryNode history_node { get; set; }
        public int graph_version { get; set; }
        public string evaluation { get; set; }
        public int mark { get; set; }
    }

    public class HistoryNode
    {
        public int node_id { get; set; }
        public string title { get; set; }
        public int cid { get; set; }
    }


}
