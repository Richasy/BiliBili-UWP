using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Video
{

    public class VideoPart
    {
        public int cid { get; set; }
        public int page { get; set; }
        public string from { get; set; }
        public string part { get; set; }
        public int duration { get; set; }
        public string vid { get; set; }
        public string weblink { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VideoPart part &&
                   cid == part.cid;
        }

        public override int GetHashCode()
        {
            return 422245175 + cid.GetHashCode();
        }
    }

}
