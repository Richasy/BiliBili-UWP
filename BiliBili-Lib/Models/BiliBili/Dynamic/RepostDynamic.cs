using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class RepostDynamic
    {
        public Owner user { get; set; }
        public RepostContent item { get; set; }
        public string origin { get; set; }
        public string origin_extend_json { get; set; }
        public OriginUser origin_user { get; set; }
        public object render_origin { get; set; }
        public string render_origin_content { get; set; }
        public class OriginUser : Author
        {
            public SlimUserInfo info { get; set; }
        }
    }

    public class RepostContent
    {
        public long rp_id { get; set; }
        public int uid { get; set; }
        public string content { get; set; }
        public string ctrl { get; set; }
        public long orig_dy_id { get; set; }
        public long pre_dy_id { get; set; }
        public int timestamp { get; set; }
        public int reply { get; set; }
        public int orig_type { get; set; }
    }

}
