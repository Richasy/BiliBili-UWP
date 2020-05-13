using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    /// <summary>
    /// 分区信息
    /// </summary>
    public class RegionContainer:Region
    {
        public string uri { get; set; }
        public int is_bangumi { get; set; }
        public List<Region> children { get; set; }
    }
    /// <summary>
    /// 分区基类
    /// </summary>
    public class Region
    {
        public int tid { get; set; }
        public int reid { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string @goto { get; set; }
        public string param { get; set; }
        public int type { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Region region &&
                   tid == region.tid;
        }

        public override int GetHashCode()
        {
            return -406633058 + tid.GetHashCode();
        }
    }
    public class RegionVideo
    {
        public string title { get; set; }
        public string cover { get; set; }
        public string uri { get; set; }
        public string param { get; set; }
        public string @goto { get; set; }
        public string name { get; set; }
        public string face { get; set; }
        public int play { get; set; }
        public int danmaku { get; set; }
        public int reply { get; set; }
        public int favourite { get; set; }
        public int pubdate { get; set; }
        public int duration { get; set; }
        public int rid { get; set; }
        public string rname { get; set; }
        public int like { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RegionVideo video &&
                   title == video.title &&
                   param == video.param;
        }

        public override int GetHashCode()
        {
            var hashCode = 1685139875;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(param);
            return hashCode;
        }
    }
}
