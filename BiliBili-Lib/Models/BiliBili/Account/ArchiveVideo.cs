using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Account
{
    public class ArchiveResponse
    {
        public int count { get; set; }
        public List<ArchiveVideo> item { get; set; }
    }
    public class ArchiveVideo
    {
        public string title { get; set; }
        public string tname { get; set; }
        public string cover { get; set; }
        public string uri { get; set; }
        public string param { get; set; }
        public string @goto { get; set; }
        public string length { get; set; }
        public int duration { get; set; }
        public bool is_popular { get; set; }
        public bool is_steins { get; set; }
        public bool is_ugcpay { get; set; }
        public bool is_cooperation { get; set; }
        public int play { get; set; }
        public int danmaku { get; set; }
        public int ctime { get; set; }
        public int ugc_pay { get; set; }
        public string author { get; set; }
        public bool state { get; set; }
        public string bvid { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ArchiveVideo video &&
                   bvid == video.bvid;
        }

        public override int GetHashCode()
        {
            return 1061302348 + EqualityComparer<string>.Default.GetHashCode(bvid);
        }
    }

}
