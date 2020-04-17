using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class Tag
    {
        public int id { get; set; }
        public string title { get; set; }
        public string uri { get; set; }
    }

    public class RegionBanner
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string hash { get; set; }
        public string uri { get; set; }
        public int resource_id { get; set; }
        public string request_id { get; set; }
        public int src_id { get; set; }
        public bool is_ad { get; set; }
        public bool is_ad_loc { get; set; }
        public int cm_mark { get; set; }
        public string client_ip { get; set; }
        public int index { get; set; }
        public int server_type { get; set; }
    }

}
