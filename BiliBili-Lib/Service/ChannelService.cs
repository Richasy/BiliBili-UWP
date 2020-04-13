using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Service
{
    public class ChannelService
    {
        private int _hotChannelOffset = 0;
        private int _squareChannelOffset = 0;
        /// <summary>
        /// 获取热门频道
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChannelBase>> GetHotChannelAsync()
        {
            _hotChannelOffset += 5;
            var param = new Dictionary<string, string>();
            param.Add("offset", _hotChannelOffset.ToString());
            string url = BiliTool.UrlContact(Api.CHANNEL_HOT, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<ChannelBase>>(url, "data.items.list");
            return data;
        }
        /// <summary>
        /// 获取频道页综合信息
        /// </summary>
        /// <returns></returns>
        public async Task<ChannelSquare> GetSquareAsync()
        {
            var param = new Dictionary<string, string>();
            _squareChannelOffset = _squareChannelOffset > 100 ? 15 : _squareChannelOffset + 15;
            param.Add("offset_rcmd", _squareChannelOffset.ToString());
            param.Add("pn", "0");
            param.Add("qn", "32");
            param.Add("spmid", "traffic.channel-square.0.0");
            string url = BiliTool.UrlContact(Api.CHANNEL_SQUARE, param, true);
            string content = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var jarr = JArray.Parse(content);
                    string subString = jarr.Where(p => p["model_type"].ToString() == "subscribe").First().SelectToken("items").ToString();
                    var subscribes = JsonConvert.DeserializeObject<List<ChannelSlim>>(subString);
                    subscribes.RemoveAll(p => p.id == 0);
                    var square = new ChannelSquare() { Subscribes = subscribes };
                    if (!string.IsNullOrEmpty(BiliTool._accessToken))
                    {
                        var scanToken = jarr.Where(p => p["model_type"].ToString() == "scaned").FirstOrDefault();
                        if (scanToken != null)
                        {
                            var scaneds = JsonConvert.DeserializeObject<List<ChannelView>>(scanToken.SelectToken("items").ToString());
                            square.Scaneds = scaneds;
                        }
                    }
                    return square;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        /// <summary>
        /// 获取频道的详细信息
        /// </summary>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public async Task<ChannelDetail> GetChannelDetailInfoAsync(int channelId)
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", channelId.ToString());
            
            string url = BiliTool.UrlContact(Api.CHANNEL_DETAIL, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<ChannelDetail>(url);
            return data;
        }
        /// <summary>
        /// 获取频道下的视频信息
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="sort">排序方式，可选值(hot,view,new)</param>
        /// <param name="offset">偏移值，初次不需要，每次请求会返回下一次请求的偏移值</param>
        /// <returns>Item1:下次偏移值;Item2:榜一视频（如果有）;Item3:视频列表</returns>
        public async Task<Tuple<string,VideoBase, List<VideoChannel>>> GetChannelVideosAsync(int channelId, string sort = "hot", string offset = "")
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", channelId.ToString());
            param.Add("sort", sort);
            if (!string.IsNullOrEmpty(offset))
                param.Add("offset", offset);
            string url = BiliTool.UrlContact(Api.CHANNEL_VIDEO, param, true);
            string content = await BiliTool.GetTextFromWebAsync(url,true);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                if (jobj["code"].ToString() == "0")
                {
                    var items = jobj["data"]["items"];
                    string nextOffset = jobj.SelectToken("data.offset").ToString();
                    VideoBase rankFirst = null;
                    if (items.First()["goto"].ToString() == "channel_detail_rank")
                    {
                        rankFirst = JsonConvert.DeserializeObject<VideoBase>(items.First().SelectToken("items[0]").ToString());
                    }
                    var videoList = JsonConvert.DeserializeObject<List<VideoChannel>>(items.ToString());
                    videoList.RemoveAll(p => p.@goto != "av");
                    return new Tuple<string, VideoBase, List<VideoChannel>>(nextOffset, rankFirst, videoList);
                }
            }
            return null;
        }

        /// <summary>
        /// 取消订阅频道
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeChannelAsync(int channelId)
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", channelId.ToString());
            var data = BiliTool.UrlContact("", param, true);
            var response = await BiliTool.PostContentToWebAsync(Api.CHANNEL_UNSUBSCRIBE, data);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                if (jobj["code"].ToString() == "0")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <returns></returns>
        public async Task<bool> SubscribeChannelAsync(int channelId)
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", channelId.ToString());
            var data = BiliTool.UrlContact("", param, true);
            var response = await BiliTool.PostContentToWebAsync(Api.CHANNEL_SUBSCRIBE, data);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                if (jobj["code"].ToString() == "0")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取频道分类索引
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChannelTab>> GetChannelTabsAsync()
        {
            var url = BiliTool.UrlContact(Api.CHANNEL_TABS, hasAccessKey: true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<ChannelTab>>(url);
            return data;
        }
        /// <summary>
        /// 获取我订阅的频道
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChannelListItem>> GetMySubscibeChannelsAsync(string offset="")
        {
            var param = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(offset))
                param.Add("offset", offset);
            var url = BiliTool.UrlContact(Api.CHANNEL_MYSUBSCRIBE,param,true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<ChannelListItem>>(url,"data.normal");
            return data;
        }
        /// <summary>
        /// 获取某分类下的频道列表
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <param name="offset">偏移值</param>
        /// <returns></returns>
        public async Task<List<ChannelListItem>> GetChannelListAsync(int id,string offset="")
        {
            var param = new Dictionary<string, string>();
            param.Add("type", id.ToString());
            if (!string.IsNullOrEmpty(offset))
                param.Add("offset", offset);
            string url = BiliTool.UrlContact(Api.CHANNEL_LIST, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<List<ChannelListItem>>(url,"data.items");
            return response;
        }
        /// <summary>
        /// 获取频道搜索结果
        /// </summary>
        /// <param name="search">搜索文本</param>
        /// <param name="pn">页码</param>
        /// <param name="ps">每页条目数</param>
        /// <returns></returns>
        public async Task<List<ChannelListItem>> GetChannelSearchResult(string search,int pn=1,int ps=20)
        {
            var param = new Dictionary<string, string>();
            param.Add("keyword", Uri.EscapeDataString(search));
            param.Add("pn", pn.ToString());
            param.Add("ps", ps.ToString());
            string url = BiliTool.UrlContact(Api.CHANNEL_SEARCH, param, true);
            var items = await BiliTool.ConvertEntityFromWebAsync<List<ChannelListItem>>(url, "data.items");
            return items;
        }
        /// <summary>
        /// 获取标签下的推荐视频
        /// </summary>
        /// <param name="tagId">标签ID</param>
        /// <param name="offset">偏移值，每次+1</param>
        /// <returns></returns>
        public async Task<List<VideoRecommend>> GetTagRecommendVideo(int tagId,int offset=1)
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", tagId.ToString());
            //param.Add("channel_name", Uri.EscapeDataString(tagName));
            param.Add("display_id", offset.ToString());
            param.Add("pull", "true");
            string url = BiliTool.UrlContact(Api.CHANNEL_TAG_RECOMMEND, param, true);
            var videos = await BiliTool.ConvertEntityFromWebAsync<List<VideoRecommend>>(url, "data.feed");
            return videos;
        }
        /// <summary>
        /// 获取标签信息
        /// </summary>
        /// <param name="tagId">标签ID</param>
        /// <returns></returns>
        public async Task<ChannelTag> GetTagDetail(int tagId)
        {
            var param = new Dictionary<string, string>();
            param.Add("channel_id", tagId.ToString());
            string url = BiliTool.UrlContact(Api.CHANNEL_TAG_TAB, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<ChannelTag>(url);
            return data;
        }
    }
}
