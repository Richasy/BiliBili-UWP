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
                                    <AdaptationSet>
                                      <ContentComponent contentType=""audio"" id=""2"" />
                                      <Representation bandwidth=""{audio.bandwidth}"" codecs=""{audio.codecs}"" id=""{audio.id}"" mimeType=""{audio.mimeType}"" >
                                        <BaseURL></BaseURL>
                                        <SegmentBase indexRange=""{audio.SegmentBase.indexRange}"">
                                          <Initialization range=""{audio.SegmentBase.Initialization}"" />
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
        /// 添加观看视频的历史记录
        /// </summary>
        /// <param name="aid">视频ID</param>
        /// <returns></returns>
        public async Task AddVideoHistoryAsync(int aid,int sid, int epid, int seconds = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            param.Add("sid", sid.ToString());
            param.Add("epid", epid.ToString());
            param.Add("realtime", seconds.ToString());
            param.Add("type", "4");
            var data = BiliTool.UrlContact("", param, true);
            await BiliTool.PostContentToWebAsync(Api.VIDEO_ADD_WATCH, data);
        }
    }
}
