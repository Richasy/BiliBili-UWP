using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class ImageDynamic
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public object[] role { get; set; }
        public object[] source { get; set; }
        public List<Picture> pictures { get; set; }
        public int pictures_count { get; set; }
        public int render_count
        {
            get => Convert.ToInt32(Math.Ceiling(pictures_count / 3.0));
        }
        public int upload_time { get; set; }
        public string at_control { get; set; }
        public int reply { get; set; }
        public Settings settings { get; set; }
        public int is_fav { get; set; }
        public class Settings
        {
            public int copy_forbidden { get; set; }
        }

        public class Picture
        {
            public string img_src { get; set; }
            public double img_width { get; set; }
            public double img_height { get; set; }
            public double img_size { get; set; }
        }
    }
}
