using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Video
{
    public class VideoSlim
    {
        public int aid { get; set; }
        public string bvid { get; set; }
        public int view { get; set; }
        public int danmaku { get; set; }
        public int reply { get; set; }
        public int favorite { get; set; }
        public int coin { get; set; }
        public int share { get; set; }
        public int like { get; set; }
        public int now_rank { get; set; }
        public int his_rank { get; set; }
        public int no_reprint { get; set; }
        public int copyright { get; set; }
        public string argue_msg { get; set; }
        public string evaluation { get; set; }
    }

}
