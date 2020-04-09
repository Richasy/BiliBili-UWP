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
    public class Region
    {
        public int tid { get; set; }
        public int reid { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string _goto { get; set; }
        public string param { get; set; }
        public string uri { get; set; }
        public int type { get; set; }
        public int is_bangumi { get; set; }
        public List<SubRegion> children { get; set; }
        public List<RegionConfig> config { get; set; }

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
    /// <summary>
    /// 子分区
    /// </summary>
    public class SubRegion
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
            return obj is SubRegion region &&
                   tid == region.tid;
        }

        public override int GetHashCode()
        {
            return -406633058 + tid.GetHashCode();
        }
    }
    /// <summary>
    /// 分区选项
    /// </summary>
    public class RegionConfig
    {
        public string scenes_name { get; set; }
        public string scenes_type { get; set; }
    }

}
