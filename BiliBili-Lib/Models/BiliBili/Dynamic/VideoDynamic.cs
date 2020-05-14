using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class VideoDynamic
    {
        public int aid { get; set; }
        public int attribute { get; set; }
        public int cid { get; set; }
        public string comment_jump_url { get; set; }
        public int copyright { get; set; }
        public int ctime { get; set; }
        public string desc { get; set; }
        public int duration { get; set; }
        public string dynamic { get; set; }
        public string jump_url { get; set; }
        public Owner owner { get; set; }
        public string pic { get; set; }
        public int pubdate { get; set; }
        public string share_subtitle { get; set; }
        public Stats stat { get; set; }
        public int state { get; set; }
        public int tid { get; set; }
        public string title { get; set; }
        public string tname { get; set; }
        public int videos { get; set; }
        public string redirect_url { get; set; }
    }

    public class Owner
    {
        public string face { get; set; }
        public int mid { get; set; }
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Owner owner &&
                   mid == owner.mid;
        }

        public override int GetHashCode()
        {
            return 1557962925 + mid.GetHashCode();
        }
    }

}
