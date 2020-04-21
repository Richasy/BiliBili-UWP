using BiliBili_Lib.Models.BiliBili.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Favorites
{
    public class FavoriteVideo
    {
        public int attr { get; set; }
        public string bv_id { get; set; }
        public string bvid { get; set; }
        public CntInfo cnt_info { get; set; }
        public Coin coin { get; set; }
        public int copyright { get; set; }
        public string cover { get; set; }
        public int ctime { get; set; }
        public int duration { get; set; }
        public int elec_open { get; set; }
        public int fav_state { get; set; }
        public int id { get; set; }
        public string intro { get; set; }
        public int like_state { get; set; }
        public string link { get; set; }
        public int page { get; set; }
        public List<VideoPart> pages { get; set; }
        public int pubtime { get; set; }
        public string short_link { get; set; }
        public int tid { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public Upper upper { get; set; }
        public class CntInfo
        {
            public int coin { get; set; }
            public int collect { get; set; }
            public int danmaku { get; set; }
            public int play { get; set; }
            public int reply { get; set; }
            public int share { get; set; }
            public int thumb_down { get; set; }
            public int thumb_up { get; set; }
        }

        public class Coin
        {
            public int coin_number { get; set; }
            public int max_num { get; set; }
        }
    }
}
