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
        public string _goto { get; set; }
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
}
