using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Anime;
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
    public class AnimeService
    {
        /// <summary>
        /// 获取动漫区块综合信息
        /// </summary>
        /// <param name="isJp"><c>true</c>代表番剧，<c>false</c>代表国创</param>
        /// <returns></returns>
        public async Task<List<AnimeModule>> GetAnimeSquareAsync(bool isJp = true)
        {
            string api = isJp ? Api.ANIME_JP_SQUARE : Api.ANIME_CHN_SQUARE;
            var url = BiliTool.UrlContact(api, null, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<AnimeModule>>(url, "result.modules");
        }
        /// <summary>
        /// 获取区块刷新信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public async Task<List<AnimeModuleItem>> GetAnimeSectionExchange(int type, int moduleId)
        {
            var param = new Dictionary<string, string>();
            param.Add("type", type.ToString());
            param.Add("oid", moduleId.ToString());
            string url = BiliTool.UrlContact(Api.ANIME_EXCHANGE, param, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<AnimeModuleItem>>(url, "result");
        }
        /// <summary>
        /// 获取番剧、电影详情
        /// </summary>
        /// <param name="epid">标识ID</param>
        /// <returns></returns>
        public async Task<BangumiDetail> GetBangumiDetailAsync(int epid,bool isEp=false)
        {
            var param = new Dictionary<string, string>();
            if(isEp)
                param.Add("ep_id", epid.ToString());
            else
                param.Add("season_id", epid.ToString());
            param.Add("season_type", "0");
            string url = BiliTool.UrlContact(Api.ANIME_DETAIL, param, true);
            return await BiliTool.ConvertEntityFromWebAsync<BangumiDetail>(url, "result");
        }
        /// <summary>
        /// 检查用户是否投币
        /// </summary>
        /// <param name="epid">标识ID</param>
        /// <returns></returns>
        public async Task<bool> CheckUserCoinAsync(int epid)
        {
            var param = new Dictionary<string, string>();
            param.Add("ep_id", epid.ToString());
            string url = BiliTool.UrlContact(Api.ANIME_CHECK_COIN, param, true);
            try
            {
                var num = await BiliTool.GetTextFromWebAsync(url, false, "result.number");
                return Convert.ToInt32(num) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 追番/追剧
        /// </summary>
        /// <param name="epid">标识ID</param>
        /// <returns></returns>
        public async Task<bool> FollowBangumiAsync(int epid)
        {
            var param = new Dictionary<string, string>();
            param.Add("season_id", epid.ToString());
            string data = BiliTool.UrlContact("", param, true);
            string content = await BiliTool.PostContentToWebAsync(Api.ANIME_FOLLOW, data);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                return jobj["message"].ToString() == "success";
            }
            return false;
        }
        /// <summary>
        /// 取消追番/追剧
        /// </summary>
        /// <param name="epid">标识ID</param>
        /// <returns></returns>
        public async Task<bool> UnfollowBangumiAsync(int epid)
        {
            var param = new Dictionary<string, string>();
            param.Add("season_id", epid.ToString());
            string data = BiliTool.UrlContact("", param, true);
            string content = await BiliTool.PostContentToWebAsync(Api.ANIME_UNFOLLOW, data);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                return jobj["message"].ToString() == "success";
            }
            return false;
        }

        /// <summary>
        /// 获取番剧播放信息
        /// </summary>
        /// <param name="aid">番剧类型标识</param>
        /// <param name="cid">视频分Pid</param>
        /// <param name="qn">分辨率ID</param>
        /// <returns></returns>
        public async Task<VideoPlayBase> GetBangumiPlayAsync(int type, int cid, int qn = 64)
        {
            var param = new Dictionary<string, string>();
            param.Add("module", "bangumi");
            param.Add("season_type", type.ToString());
            param.Add("cid", cid.ToString());
            param.Add("qn", qn.ToString());
            param.Add("fnver", "0");
            param.Add("otype", "json");
            param.Add("fnval", "16");
            string url = BiliTool.UrlContact(Api.ANIME_PLAY, param, true, true);
            var data = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(data))
            {
                var jobj = JObject.Parse(data);
                if (jobj.ContainsKey("dash"))
                {
                    return JsonConvert.DeserializeObject<VideoPlayDash>(data);
                }
                else if (jobj.ContainsKey("durl"))
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
                                        <SegmentBase indexRange=""{video.SegmentBase.indexRange}"">
                                          <Initialization range=""{video.SegmentBase.Initialization}"" />
                                        </SegmentBase>
                                      </Representation>
                                    </AdaptationSet>
                                    {{audio}}
                                  </Period>
                                </MPD>
                                ";
                if (audio == null)
                    mpdStr = mpdStr.Replace("{audio}", "");
                else
                    mpdStr = mpdStr.Replace("{audio}", $@"<AdaptationSet>
                                      <ContentComponent contentType=""audio"" id=""2"" />
                                      <Representation bandwidth=""{audio.bandwidth}"" codecs=""{audio.codecs}"" id=""{audio.id}"" mimeType=""{audio.mimeType}"" >
                                        <BaseURL></BaseURL>
                                        <SegmentBase indexRange=""{audio.SegmentBase.indexRange}"">
                                          <Initialization range=""{audio.SegmentBase.Initialization}"" />
                                        </SegmentBase>
                                      </Representation>
                                    </AdaptationSet>");
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(mpdStr)).AsInputStream();
                var soure = await AdaptiveMediaSource.CreateFromStreamAsync(stream, new Uri(video.baseUrl), "application/dash+xml", httpClient);
                var s = soure.Status;
                soure.MediaSource.DownloadRequested += (sender, args) =>
                {
                    if (args.ResourceContentType == "audio/mp4" && audio!=null)
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
        /// 添加观看视频的历史记录
        /// </summary>
        /// <param name="aid">视频ID</param>
        /// <returns></returns>
        public async Task AddVideoHistoryAsync(int aid,int cid, int epid, int seconds = 0,int sid=0)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            param.Add("cid", cid.ToString());
            param.Add("sid", sid.ToString());
            param.Add("epid", epid.ToString());
            param.Add("progress", seconds.ToString());
            param.Add("realtime", seconds.ToString());
            param.Add("type", "4");
            var data = BiliTool.UrlContact("", param, true);
            await BiliTool.PostContentToWebAsync(Api.VIDEO_ADD_WATCH, data);
        }
        /// <summary>
        /// 获取动漫索引限制条件
        /// </summary>
        /// <param name="type">分区类型</param>
        /// <returns></returns>
        public async Task<IndexCondition> GetBangumiIndexConditionAsync(int type)
        {
            var param = new Dictionary<string, string>();
            param.Add("season_type", type.ToString());
            param.Add("type", "0");
            string url = BiliTool.UrlContact(Api.ANIME_INDEX_CONDITION, param, true);
            var result = await BiliTool.ConvertEntityFromWebAsync<IndexCondition>(url);
            return result;
        }
        /// <summary>
        /// 获取动漫索引筛查结果
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="type">分区类型</param>
        /// <param name="conditions">限制条件</param>
        /// <returns></returns>
        public async Task<Tuple<bool,List<AnimeIndexResult>>> GetBangumiIndexResultsAsync(int page,int type,List<KeyValueModel> conditions)
        {
            var param = new Dictionary<string, string>();
            foreach (var item in conditions)
            {
                param.Add(item.Key, item.Value);
            }
            param.Add("type", "0");
            param.Add("page", page.ToString());
            param.Add("season_type", type.ToString());
            param.Add("pagesize", "21");
            string url = BiliTool.UrlContact(Api.ANIME_INDEX_RESULT, param, true);
            var response = await BiliTool.GetTextFromWebAsync(url);
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var jobj = JObject.Parse(response);
                    bool hasNext = jobj["has_next"].ToString() == "1";
                    var data = JsonConvert.DeserializeObject<List<AnimeIndexResult>>(jobj["list"].ToString());
                    return new Tuple<bool, List<AnimeIndexResult>>(hasNext, data);
                }
            }
            catch (Exception)
            {
            }
           
            return null;
        }

        /// <summary>
        /// 获取动漫时间线
        /// </summary>
        /// <param name="type">区块类型，番剧：2，国创：3</param>
        /// <returns></returns>
        public async Task<List<Timeline>> GetTimelineAsync(int type)
        {
            var param = new Dictionary<string, string>();
            param.Add("type", type.ToString());
            param.Add("filter_type", "0");
            string url = BiliTool.UrlContact(Api.ANIME_TIMELINE, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<Timeline>>(url, "result.data");
            return data;
        }
        /// <summary>
        /// 不喜欢某番剧
        /// </summary>
        /// <param name="arg">参数</param>
        /// <returns></returns>
        public async Task<bool> DislikeRecommendVideoAsync(string bangumiId)
        {
            var param = new Dictionary<string, string>();
            param.Add("id", bangumiId);
            param.Add("goto", "bangumi");
            param.Add("reason_id", "1");
            var url = BiliTool.UrlContact(Api.VIDEO_RECOMMEND_DISLIKE, param, true);
            var response = await BiliTool.GetTextFromWebAsync(url, true);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }
        /// <summary>
        /// 转发动漫/电影/电视剧等
        /// </summary>
        /// <param name="content">转发内容</param>
        /// <param name="videoId">视频ID</param>
        /// <param name="atIds">At的人</param>
        /// <param name="typeName">番剧：4097,影视:4098,电视剧:4099,国创:4100</param>
        /// <returns></returns>
        public async Task<bool> RepostBangumiAsync(string content, int epId, string typeName, List<RepostLocation> atIds)
        {
            var param = new Dictionary<string, string>();
            string type = "";
            switch (typeName)
            {
                case "番剧":
                    type = "4097";
                    break;
                case "影视":
                    type = "4098";
                    break;
                case "电视剧":
                case "综艺":
                    type = "4099";
                    break;
                case "国创":
                    type = "4100";
                    break;
                case "纪录片":
                    type = "4101";
                    break;
                default:
                    type = "4097";
                    break;
            }
            param.Add("content", Uri.EscapeDataString(content));
            param.Add("at_uids", string.Join(',', atIds.Select(p => p.data)));
            param.Add("ctrl", Uri.EscapeDataString(JsonConvert.SerializeObject(atIds)));
            param.Add("share_uid", BiliTool.mid.ToString());
            param.Add("rid", epId.ToString());
            param.Add("type", type);
            param.Add("repost_code", "20000");
            param.Add("sync_comment", "0");
            param.Add("sketch", "");
            param.Add("uid", "0");
            param.Add("share_info", "");
            param.Add("extension", Uri.EscapeDataString("{\"emoji_type\":1}"));
            param.Add("statistics", Uri.EscapeDataString("{\"appId\":1,\"version\":\"5.56.1\",\"abtest\":\"\",\"platform\":1}"));
            var req = BiliTool.UrlContact("", param, true);
            var response = await BiliTool.PostContentToWebAsync(Api.VIDEO_REPOST, req);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }
    }
}
