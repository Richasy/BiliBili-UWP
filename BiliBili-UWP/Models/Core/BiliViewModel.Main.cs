using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.Core
{
    public partial class BiliViewModel : NotifyPropertyBase
    {
        public ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        public ObservableCollection<VideoRecommend> RecommendVideoCollection = new ObservableCollection<VideoRecommend>();
        public BiliBiliClient _client;
        public BiliWebClient _webClient;
        public BiliSecure _secure;
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
            string _userId = AppTool.GetLocalSetting(Settings.UserId, "");
            _client = new BiliBiliClient(_access, _refresh, _userId, _expiry);
            _client.Account.TokenChanged += TokenChanged;
        }

        public void TimerInit()
        {
            _channelChangeTimer.Tick += ChannelChangTimer_Tick;
            _channelChangeTimer.Start();
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
            regions.Where(p=>p.children!=null).ToList().ForEach(p => RegionCollection.Add(p));
        }
    }
}
