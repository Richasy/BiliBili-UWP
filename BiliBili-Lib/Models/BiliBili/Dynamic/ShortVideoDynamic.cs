using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class ShortVideoDynamic
    {
        public User user { get; set; }
        public Item item { get; set; }
        public class User
        {
            public string uid { get; set; }
            public string head_url { get; set; }
            public int is_vip { get; set; }
            public string name { get; set; }
        }
        public class Item
        {
            public int id { get; set; }
            public Cover cover { get; set; }
            public string[] tags { get; set; }
            public string description { get; set; }
            public int video_time { get; set; }
            public string upload_time_text { get; set; }
            public string upload_time { get; set; }
            public string at_control { get; set; }
            public string video_playurl { get; set; }
            public string[] backup_playurl { get; set; }
            public string video_size { get; set; }
            public string verify_status { get; set; }
            public string verify_status_text { get; set; }
            public int reply { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int watched_num { get; set; }
        }

        public class Cover
        {
            public string _default { get; set; }
            public string unclipped { get; set; }
        }
    }

}
