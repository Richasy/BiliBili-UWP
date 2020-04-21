using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class WebDynamic
    {
        public long rid { get; set; }
        public SlimUserInfo user { get; set; }
        public Vest vest { get; set; }
        public Sketch sketch { get; set; }
        public class Vest
        {
            public int uid { get; set; }
            public string content { get; set; }
            public string ctrl { get; set; }
        }

        public class Sketch
        {
            public string title { get; set; }
            public string desc_text { get; set; }
            public string cover_url { get; set; }
            public string target_url { get; set; }
            public long sketch_id { get; set; }
            public int biz_type { get; set; }
        }
    }
}
