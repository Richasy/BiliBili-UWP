using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.Others
{
    public class RepostLocation
    {
        public int location { get; set; }
        public int type { get; set; }
        public string data { get; set; }
        public int length { get; set; }
        public RepostLocation()
        {

        }
        public RepostLocation(int loc,int leng,int id)
        {
            type = 1;
            location = loc;
            length = leng;
            data = id.ToString();
        }
    }
}
