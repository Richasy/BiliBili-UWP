using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Streaming.Adaptive;
using Windows.Web.Http;

namespace BiliBili_Lib.Service
{
    public class VideoService
    {
        public VideoService()
        {

        }
        /// <summary>
        /// 加载默认的子分区视频内容
        /// </summary>
        /// <param name="rid">分区ID</param>
        /// <param name="ctime">偏移值</param>
        /// <returns></returns>
        public async Task<Tuple<int,List<RegionVideo>>> GetSubRegionDefaultAsync(int rid,int ctime=0)
        {
            var param = new Dictionary<string, string>();
            param.Add("rid", rid.ToString());
            param.Add("pull", "0");
            string api = "";
            if (ctime == 0)
                api = Api.REGION_DYNAMIC_CHILD_INIT;
            else
            {
                api = Api.REGION_DYNAMIC_CHILD_REFRESH;
                param.Add("ctime", ctime.ToString());
            }
            string url = BiliTool.UrlContact(api, param, true);
            string content = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                var list = new List<RegionVideo>();
                if (jobj.ContainsKey("recommend"))
                {
                    var rec = JsonConvert.DeserializeObject<List<RegionVideo>>(jobj["recommend"].ToString());
                    rec.ForEach(p => list.Add(p));
                }
                if (jobj.ContainsKey("new"))
                {
                    var news= JsonConvert.DeserializeObject<List<RegionVideo>>(jobj["new"].ToString());
                    news.ForEach(p => list.Add(p));
                }
                var result = new Tuple<int, List<RegionVideo>>(Convert.ToInt32(jobj["cbottom"]),list);
                return result;
            }
            return null;
        }
        /// <summary>
        /// 加载排序后的分区子内容
        /// </summary>
        /// <param name="rid">分区ID</param>
        /// <param name="order">排序方式</param>
        /// <param name="pn">页码</param>
        /// <returns></returns>
        public async Task<List<RegionVideo>> GetSubRegionSortVideoAsync(int rid,string order,int pn=1)
        {
            var param = new Dictionary<string, string>();
            param.Add("rid", rid.ToString());
            param.Add("pn", pn.ToString());
            param.Add("ps", "30");
            param.Add("order", order);
            string url = BiliTool.UrlContact(Api.REGION_DYNAMIC_CHILD_SORT, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<RegionVideo>>(url);
            return data;
        }
        /// <summary>
        /// 获取分区排行榜数据
        /// </summary>
        /// <param name="rid">分区ID</param>
        /// <returns></returns>
        public async Task<List<WebVideo>> GetRegionRankAsync(int rid)
        {
            var param = new Dictionary<string, string>();
            param.Add("rid", rid.ToString());
            string url = BiliTool.UrlContact(Api.REGION_RANK, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<WebVideo>>(url);
            return data;
        }
        /// <summary>
        /// 加载默认的分区内容
        /// </summary>
        /// <param name="rid">分区ID</param>
        /// <param name="ctime">偏移值</param>
        /// <returns></returns>
        public async Task<Tuple<List<RegionBanner>,int,List<RegionVideo>>> GetRegionSquareAsync(int rid,int ctime=0)
        {
            var param = new Dictionary<string, string>();
            param.Add("rid", rid.ToString());
            param.Add("pull", "0");
            string api = "";
            if (ctime == 0)
                api = Api.REGION_DYNAMIC_INIT;
            else
            {
                api = Api.REGION_DYNAMIC_REFRESH;
                param.Add("ctime", ctime.ToString());
            }
            string url = BiliTool.UrlContact(api, param, true);
            string content = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                var list = new List<RegionVideo>();
                List<RegionBanner> banner = new List<RegionBanner>();
                if (jobj.ContainsKey("banner"))
                {
                    var ban = JsonConvert.DeserializeObject<List<RegionBanner>>(jobj["banner"]["top"].ToString());
                    ban.ForEach(p => banner.Add(p));
                }
                if (jobj.ContainsKey("recommend"))
                {
                    var rec = JsonConvert.DeserializeObject<List<RegionVideo>>(jobj["recommend"].ToString());
                    rec.ForEach(p => list.Add(p));
                }
                if (jobj.ContainsKey("new"))
                {
                    var news = JsonConvert.DeserializeObject<List<RegionVideo>>(jobj["new"].ToString());
                    news.ForEach(p => list.Add(p));
                }
                var result = new Tuple<List<RegionBanner>, int, List<RegionVideo>>(banner,Convert.ToInt32(jobj["cbottom"]), list);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 获取视频分P
        /// </summary>
        /// <param name="aid">视频avid</param>
        /// <returns></returns>
        public async Task<List<VideoPart>> GetVideoPartsAsync(int aid)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            string url = BiliTool.UrlContact(Api.VIDEO_PART,param,true);
            var parts = await BiliTool.ConvertEntityFromWebAsync<List<VideoPart>>(url);
            return parts;
        }
        /// <summary>
        /// 获取视频详细信息（包括分P）
        /// </summary>
        /// <param name="aid">视频avid</param>
        /// <returns></returns>
        public async Task<VideoDetail> GetVideoDetailAsync(int aid)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            string url = BiliTool.UrlContact(Api.VIDEO_DETAIL_INFO, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<VideoDetail>(url);
            return data;
        }
        /// <summary>
        /// 获取视频相关
        /// </summary>
        /// <param name="aid">视频avid</param>
        /// <returns></returns>
        public async Task<List<VideoDetail>> GetVideoRelatedAsync(int aid)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            string url = BiliTool.UrlContact(Api.VIDEO_RELATED, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoDetail>>(url);
            return data;
        }

        /// <summary>
        /// 获取视频播放信息
        /// </summary>
        /// <param name="aid">视频avid</param>
        /// <param name="cid">视频分Pid</param>
        /// <param name="qn">分辨率ID</param>
        /// <returns></returns>
        public async Task<VideoPlayBase> GetVideoPlayAsync(int aid,int cid,int qn=64)
        {
            var param = new Dictionary<string, string>();
            param.Add("avid", aid.ToString());
            param.Add("cid", cid.ToString());
            param.Add("qn", qn.ToString());
            param.Add("mid", BiliTool.mid);
            param.Add("fnver", "0");
            param.Add("fnval", "16");
            string url = BiliTool.UrlContact(Api.VIDEO_PLAY, param, true);
            var data = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(data))
            {
                var jobj = JObject.Parse(data);
                if (jobj.ContainsKey("dash"))
                {
                    return JsonConvert.DeserializeObject<VideoPlayDash>(data);
                }
                else if(jobj.ContainsKey("durl"))
                {
                    return JsonConvert.DeserializeObject<VideoPlayFlv>(data);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据分片条目创建媒体来源
        /// </summary>
        /// <param name="video">视频</param>
        /// <param name="audio">音频</param>
        /// <returns></returns>
        public async Task<MediaSource> CreateMediaSourceAsync(VideoDashItem video, VideoDashItem audio)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Referer = new Uri("https://www.bilibili.com");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                var mpdStr = $@"<MPD xmlns=""urn:mpeg:DASH:schema:MPD:2011""  profiles=""urn:mpeg:dash:profile:isoff-on-demand:2011"" type=""static"">
                                  <Period  start=""PT0S"">
                                    <AdaptationSet>
                                      <ContentComponent contentType=""video"" id=""1"" />
                                      <Representation bandwidth=""{video.bandwidth}"" codecs=""{video.codecs}"" height=""{video.height}"" id=""{video.id}"" mimeType=""{video.mimeType}"" width=""{video.width}"">
                                        <BaseURL></BaseURL>
                                        <SegmentBase indexRange=""{video.segment_base.index_range}"">
                                          <Initialization range=""{video.segment_base.initialization}"" />
                                        </SegmentBase>
                                      </Representation>
                                    </AdaptationSet>
                                    <AdaptationSet>
                                      <ContentComponent contentType=""audio"" id=""2"" />
                                      <Representation bandwidth=""{audio.bandwidth}"" codecs=""{audio.codecs}"" id=""{audio.id}"" mimeType=""{audio.mimeType}"" >
                                        <BaseURL></BaseURL>
                                        <SegmentBase indexRange=""{audio.segment_base.index_range}"">
                                          <Initialization range=""{audio.segment_base.initialization}"" />
                                        </SegmentBase>
                                      </Representation>
                                    </AdaptationSet>
                                  </Period>
                                </MPD>
                                ";
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(mpdStr)).AsInputStream();
                var soure = await AdaptiveMediaSource.CreateFromStreamAsync(stream, new Uri(video.baseUrl), "application/dash+xml", httpClient);
                var s = soure.Status;
                soure.MediaSource.DownloadRequested += (sender, args) =>
                {
                    if (args.ResourceContentType == "audio/mp4")
                    {
                        args.Result.ResourceUri = new Uri(audio.baseUrl);
                    }
                };
                return MediaSource.CreateFromAdaptiveMediaSource(soure.MediaSource);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 检查视频状态（是否投币，点赞，收藏等）
        /// </summary>
        /// <param name="aid">视频ID</param>
        /// <param name="type">检查类型</param>
        /// <returns></returns>
        public async Task<bool> CheckVideoStatusAsync(int aid,string type="like")
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            string url = "";
            switch (type)
            {
                case "like":
                    url = Api.VIDEO_IS_LIKE;
                    break;
                case "coin":
                    url = Api.VIDEO_IS_COIN;
                    break;
                case "favorite":
                    url = Api.VIDEO_IS_FAVORITE;
                    break;
            }
            url = BiliTool.UrlContact(url, param, true);
            var content = await BiliTool.GetTextFromWebAsync(url);
            if(type=="like")
                return content=="1";
            else
            {
                var jobj = JObject.Parse(content);
                if (type == "coin")
                    return jobj["multiply"].ToString() == "1";
                else if (type == "favorite")
                    return Convert.ToBoolean(jobj["favoured"]);
            }
            return false;
        }
    }
}
