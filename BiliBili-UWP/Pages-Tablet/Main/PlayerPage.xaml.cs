using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        private TabletVideoDetailBlock _videoBlock = App.AppViewModel.CurrentVideoDetailBlock;
        private TabletBangumiDetailBlock _bangumiBlock = App.AppViewModel.CurrentBangumiDetailBlock;
        public PlayerPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null)
            {
                if (e.Parameter is int aid)
                {
                    await InitVideo(aid, 0, "");
                }
                else if (e.Parameter is Tuple<int, bool> bangumiData)
                {
                    await InitBangumi(bangumiData.Item1, bangumiData.Item2);
                }
                else if (e.Parameter is VideoActiveArgs args)
                {
                    await InitVideo(args.aid, args.cid, args.bvid);
                }
                else if(e.Parameter is Tuple<int,List<VideoDetail>> listData)
                {
                    await InitPlayList(listData.Item1, listData.Item2);
                }
            }
            base.OnNavigatedTo(e);
        }
        private async Task InitPlayList(int startId, List<VideoDetail> list)
        {
            if (!Container.Children.Contains(_videoBlock))
            {
                Container.Children.Add(_videoBlock);
            }
            _videoBlock.Visibility = Visibility.Visible;
            await _videoBlock.Init(startId, list);
        }
        private async Task InitVideo(int aid, int cid, string bvid)
        {
            if (!Container.Children.Contains(_videoBlock))
            {
                Container.Children.Add(_videoBlock);
            }
            _videoBlock.Visibility = Visibility.Visible;
            try
            {
                await _videoBlock.Init(aid, cid, bvid);
            }
            catch (InvalidDataException)
            {
                Container.Children.Remove(_videoBlock);
                _videoBlock.Visibility = Visibility.Collapsed;
                var result = BiliTool.GetResultFromUri(_videoBlock._detail.redirect_url);
                await InitBangumi(Convert.ToInt32(result.Param), true);
            }
        }
        private async Task InitBangumi(int epId, bool isEp)
        {
            if (!Container.Children.Contains(_bangumiBlock))
            {
                Container.Children.Add(_bangumiBlock);
            }
            _bangumiBlock.Visibility = Visibility.Visible;
            await _bangumiBlock.Init(epId, isEp);
            
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Container.Children.Clear();
            TabletMainPage.Current.HideBackgroundImage();
            App.AppViewModel.CurrentVideoPlayer = null;
            _videoBlock.Visibility = Visibility.Collapsed;
            _bangumiBlock.Visibility = Visibility.Collapsed;
            _videoBlock.ClearPlayList();
            _videoBlock.VideoPlayer.Close();
            _bangumiBlock.VideoPlayer.Close();
            base.OnNavigatingFrom(e);
        }
    }
}
