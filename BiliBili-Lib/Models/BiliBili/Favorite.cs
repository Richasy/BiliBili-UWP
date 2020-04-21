using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{

    public class FavoriteItem
    {
        public int attr { get; set; }
        public FavoriteInfo cnt_info { get; set; }
        public string cover { get; set; }
        public int cover_type { get; set; }
        public int ctime { get; set; }
        public int fav_state { get; set; }
        public int fid { get; set; }
        public int id { get; set; }
        public string intro { get; set; }
        public int like_state { get; set; }
        public int media_count { get; set; }
        public int mid { get; set; }
        public int mtime { get; set; }
        public int state { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public Upper upper { get; set; }
    }

    public class FavoriteInfo
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
    public class Upper
    {
        public string face { get; set; }
        public int followed { get; set; }
        public int mid { get; set; }
        public string name { get; set; }
        public int vip_pay_type { get; set; }
        public int vip_statue { get; set; }
        public int vip_type { get; set; }
    }

}
