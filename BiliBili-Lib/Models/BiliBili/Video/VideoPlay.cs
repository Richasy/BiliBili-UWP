using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Video
{
    public class VideoPlayBase
    {
        public string from { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public int quality { get; set; }
        public string format { get; set; }
        public int timelength { get; set; }
        public string accept_format { get; set; }
        public List<string> accept_description { get; set; }
        public List<int> accept_quality { get; set; }
        public int video_codecid { get; set; }
        public string seek_param { get; set; }
        public string seek_type { get; set; }
    }
    public class VideoPlayDash : VideoPlayBase
    {
        public VideoDash dash { get; set; }
    }
    public class VideoPlayFlv : VideoPlayBase
    {
        public List<VideoDurl> durl { get; set; }
    }

    public class VideoDurl
    {
        public int order { get; set; }
        public int length { get; set; }
        public int size { get; set; }
        public string ahead { get; set; }
        public string vhead { get; set; }
        public string url { get; set; }
        public List<string> backup_url { get; set; }
    }


    public class VideoDash
    {
        public int duration { get; set; }
        public float minBufferTime { get; set; }
        public float min_buffer_time { get; set; }
        public List<VideoDashItem> video { get; set; }
        public List<VideoDashItem> audio { get; set; }
    }

    public class VideoDashItem
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
        public string base_url { get; set; }
        public string[] backupUrl { get; set; }
        public string[] backup_url { get; set; }
        public int bandwidth { get; set; }
        public string mimeType { get; set; }
        public string mime_type { get; set; }
        public string codecs { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string frameRate { get; set; }
        public string frame_rate { get; set; }
        public string sar { get; set; }
        public int startWithSap { get; set; }
        public int start_with_sap { get; set; }
        public SegmentBase segment_base { get; set; }
        public SegmentBase SegmentBase { get; set; }
        public int codecid { get; set; }
    }

    public class SegmentBase
    {
        public string initialization { get; set; }
        public string Initialization { get; set; }
        public string index_range { get; set; }
        public string indexRange { get; set; }
    }

}
