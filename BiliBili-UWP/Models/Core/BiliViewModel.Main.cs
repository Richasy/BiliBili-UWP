using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI.Others;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BiliBili_UWP.Models.Core
{
    public partial class BiliViewModel : NotifyPropertyBase
    {
        public ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        public ObservableCollection<VideoRecommend> RecommendVideoCollection = new ObservableCollection<VideoRecommend>();
        public BiliBiliClient _client;
        public BiliWebClient _webClient;
        public BiliSecure _secure;
        public List<EmojiItem> _emojis = new List<EmojiItem>();
        public BiliViewModel()
        {
            ClientInit();
            TimerInit();
        }

        public void ClientInit()
        {
            _webClient = new BiliWebClient();
            _secure = new BiliSecure();
            string _access = AppTool.GetLocalSetting(Settings.AccessToken, "");
            string _refresh = AppTool.GetLocalSetting(Settings.RefreshToken, "");
            int _expiry = Convert.ToInt32(AppTool.GetLocalSetting(Settings.TokenExpiry, "0"));
            _client = new BiliBiliClient(_access, _refresh, _expiry);
            _client.Account.TokenChanged += TokenChanged;
        }

        public void TimerInit()
        {
            _channelChangeTimer.Tick += ChannelChangTimer_Tick;
            _channelChangeTimer.Start();
            _messageTimer.Tick += MessageTimer_Tick;
            _messageTimer.Start();
        }

        private void TokenChanged(object sender, TokenPackage e)
        {
            AppTool.WriteLocalSetting(Settings.AccessToken, e.AccessToken);
            AppTool.WriteLocalSetting(Settings.RefreshToken, e.RefreshToken);
            AppTool.WriteLocalSetting(Settings.TokenExpiry, e.Expiry.ToString());
            AppTool.WriteLocalSetting(Settings.UserId, "");
        }

        /// <summary>
        /// 获取分区
        /// </summary>
        /// <returns></returns>
        public async Task GetRegionsAsync()
        {
            RegionCollection.Clear();
            var regions = await _client.GetRegionIndexAsync();
            if (regions != null)
            {
                await IOTool.SetLocalDataAsync("region.json", JsonConvert.SerializeObject(regions.Where(p => p.children != null).ToList()));
            }
            else
            {
                regions = await IOTool.GetLocalDataAsync<List<RegionContainer>>("region.json");
            }
            regions.Where(p => p.children != null).ToList().ForEach(p => RegionCollection.Add(p));
        }

        /// <summary>
        /// 转换动态信息
        /// </summary>
        /// <param name="type">类型ID</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public object DynamicContentConvert(int type, string content)
        {
            if (type == 1)
                return JsonConvert.DeserializeObject<RepostDynamic>(content);
            else if (type == 2)
            {
                var jobj = JObject.Parse(content);
                if (jobj.ContainsKey("item"))
                    return JsonConvert.DeserializeObject<ImageDynamic>(jobj["item"].ToString());
                else
                    return JsonConvert.DeserializeObject<ImageDynamic>(content);
            }
            else if (type == 4)
            {
                var jobj = JObject.Parse(content);
                if (jobj.ContainsKey("item"))
                    return JsonConvert.DeserializeObject<TextDynamic>(jobj["item"].ToString());
                else
                    return JsonConvert.DeserializeObject<TextDynamic>(content);
            }
            else if (type == 8)
                return JsonConvert.DeserializeObject<VideoDynamic>(content);
            else if (type == 64)
                return JsonConvert.DeserializeObject<DocumentDynamic>(content);
            else if (type == 512 || type == 4101)
                return JsonConvert.DeserializeObject<AnimeDynamic>(content);
            else if (type == 16)
                return JsonConvert.DeserializeObject<ShortVideoDynamic>(content);
            else if (type == 2048)
                return JsonConvert.DeserializeObject<WebDynamic>(content);
            else if (type == 4303)
                return JsonConvert.DeserializeObject<CourseDynamic>(content);
            else if (type == 256)
                return JsonConvert.DeserializeObject<MusicDynamic>(content);
            else if (type == 4200)
                return JsonConvert.DeserializeObject<LiveDynamic>(content);
            return null;
        }

        /// <summary>
        /// Emoji表情初始化
        /// </summary>
        /// <param name="isRefresh">是否强制刷新</param>
        /// <returns></returns>
        public async Task InitEmoji(bool isRefresh = false)
        {
            bool isInit = AppTool.GetBoolSetting(Settings.IsInitEmoji, false);
            if (!isInit || isRefresh)
            {
                var emojis = await _client.GetTotalEmojiAsync();
                if (emojis != null && emojis.Count > 0)
                {
                    var emo = new List<EmojiItem>();
                    foreach (var item in emojis)
                    {
                        emo = emo.Concat(item.emojis).ToList();
                    }
                    _emojis = emo;
                    AppTool.WriteLocalSetting(Settings.IsInitEmoji, "True");
                    await IOTool.SetLocalDataAsync("emoji.json", JsonConvert.SerializeObject(_emojis));
                }
            }
            else
            {
                var emojis = await IOTool.GetLocalDataAsync<List<EmojiItem>>("emoji.json");
                _emojis = emojis;
            }
        }

        public async Task AddViewLater(object sender, int id)
        {
            var LaterViewButton = sender as AppBarButton;
            LaterViewButton.IsEnabled = false;
            bool result = await App.BiliViewModel._client.Account.AddViewLaterAsync(id);
            if (result)
            {
                new TipPopup("已添加至稍后观看").ShowMessage();
            }
            else
            {
                new TipPopup("添加失败").ShowError();
            }
            LaterViewButton.IsEnabled = true;
        }
    }
}
