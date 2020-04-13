using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class TextDynamic
    {
        public int uid { get; set; }
        public string content { get; set; }
        public string ctrl { get; set; }
        public int orig_dy_id { get; set; }
        public int pre_dy_id { get; set; }
        public int timestamp { get; set; }
        public int reply { get; set; }
    }

}
