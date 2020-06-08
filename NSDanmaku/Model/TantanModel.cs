using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSDanmaku.Model
{
    public class TantanSearchModel
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public int errorCode { get; set; }
        public bool hasMore { get; set; }
        public List<animes> animes { get; set; }
    }
    public class animes
    {
        public int animeId { get; set; }
        public string animeTitle { get; set; }
        public int type { get; set; }
        public List<episodes> episodes { get; set; }
    }
    public class episodes
    {
        public int episodeId { get; set; }
        public string episodeTitle { get; set; }
        public string animeTitle { get; set; }
    }

    public class CommentModel
    {
        public int count { get; set; }
        public List<comments> comments { get; set; }
    }
    public class comments
    {
        public long cid { get; set; }
        public string m { get; set; }
        public string p { get; set; }
    }
}
