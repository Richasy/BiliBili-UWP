using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Layout;
using BiliBili_UWP.Components.Others;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using BiliBili_UWP.Models.UI.Others;
using BiliBili_UWP.Pages.Main;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;

namespace BiliBili_UWP.Models.Core
{
    public class AppViewModel
    {
        public double StateChangeWidth = 1000;
        public SideMenuItem SelectedSideMenuItem { get; set; }
        public Type CurrentPageType { get; set; }
        public SidePanel CurrentSidePanel;
        public PagePanel CurrentPagePanel;
        public VideoPlayer CurrentVideoPlayer;
        public PlayerType CurrentPlayerType;
        public WebPopup _webPopup;
        public DocumentPopup _documentPopup;
        public ReplyDetailPopup _replyDetailPopup;
        public UpdatePopup _updatePopup;

        public List<Tuple<Guid, Action<Size>>> WindowsSizeChangedNotify { get; set; } = new List<Tuple<Guid, Action<Size>>>();
        public ObservableCollection<SystemFont> FontCollection = new ObservableCollection<SystemFont>();
        public AppViewModel()
        {
            Window.Current.SizeChanged += WindowSizeChangedHandle;
        }
        public void WindowSizeChangedHandle(object sender, WindowSizeChangedEventArgs e)
        {
            if (WindowsSizeChangedNotify.Count > 0)
            {
                WindowsSizeChangedNotify.ForEach(p => p.Item2?.Invoke(e.Size));
            }
        }
        public void AppInitByActivated(string argument)
        {
            QueryString args = QueryString.Parse(argument);
            args.TryGetValue("action", out string action);

        }
        public async void CheckAppUpdate()
        {
            string localVersion = AppTool.GetLocalSetting(Settings.LocalVersion, "");
            string nowVersion = string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
            if (localVersion != nowVersion)
            {
                var updateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Others/Update.txt"));
                string updateInfo = await FileIO.ReadTextAsync(updateFile);
                if (_updatePopup == null)
                    _updatePopup = new UpdatePopup(updateInfo);
                _updatePopup.ShowPopup();
                AppTool.WriteLocalSetting(Settings.LocalVersion, nowVersion);
            }
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="aid">AV号</param>
        /// <param name="sender">触发控件（用于查找封面以实现连接动画）</param>
        /// <param name="fromSign">来源参数</param>
        public void PlayVideo(int aid, object sender = null, string fromSign = "")
        {
            if (CurrentPagePanel.IsSubPageOpen)
                CurrentPagePanel.IsSubPageOpen = false;
            SelectedSideMenuItem = null;
            if (sender != null)
            {
                var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("VideoConnectedAnimation", image);
            }
            CurrentSidePanel.SetSelectedItem(SideMenuItemType.Line);
            CurrentPagePanel.NavigateToPage(SideMenuItemType.VideoPlayer, new Tuple<int, string>(aid, fromSign));
        }
        /// <summary>
        /// 播放番剧
        /// </summary>
        /// <param name="epid">AV号</param>
        /// <param name="sender">触发控件（用于查找封面以实现连接动画）</param>
        public void PlayBangumi(int epid, object sender = null, bool isEp = false)
        {
            if (CurrentPagePanel.IsSubPageOpen)
                CurrentPagePanel.IsSubPageOpen = false;
            SelectedSideMenuItem = null;
            if (sender != null)
            {
                var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("VideoConnectedAnimation", image);
            }
            CurrentSidePanel.SetSelectedItem(SideMenuItemType.Line);
            if (isEp)
                CurrentPagePanel.NavigateToPage(SideMenuItemType.BangumiPlayer, new Tuple<int, bool>(epid, isEp));
            else
                CurrentPagePanel.NavigateToPage(SideMenuItemType.BangumiPlayer, epid);
        }
        /// <summary>
        /// 获取当前的播放页
        /// </summary>
        /// <returns></returns>
        public IPlayerPage GetCurrentPlayerPage()
        {
            IPlayerPage page = null;
            switch (CurrentPlayerType)
            {
                case PlayerType.Video:
                    page = VideoPage.Current;
                    break;
                case PlayerType.Bangumi:
                    page = BangumiPage.Current;
                    break;
                case PlayerType.Live:
                    break;
                default:
                    break;
            }
            return page;
        }
        /// <summary>
        /// 进入全屏模式
        /// </summary>
        /// <param name="isFull">是否为全屏模式</param>
        public void PlayVideoFullScreen(bool isFull)
        {
            IPlayerPage page = GetCurrentPlayerPage();
            if (isFull)
            {
                page.RemovePlayer();
                MainPage.Current.InsertPlayer();
                CurrentVideoPlayer.DanmakuBarVisibility = Visibility.Collapsed;
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
                if (!CurrentVideoPlayer.MTC.IsCinema)
                {
                    MainPage.Current.RemovePlayer();
                    page.InsertPlayer();
                    CurrentVideoPlayer.DanmakuBarVisibility = Visibility.Visible;
                }
            }
        }
        /// <summary>
        /// 进入影院模式
        /// </summary>
        /// <param name="isCinema">是否为影院模式</param>
        public void PlayVideoCinema(bool isCinema)
        {
            IPlayerPage page = GetCurrentPlayerPage();
            if (isCinema)
            {
                page.RemovePlayer();
                MainPage.Current.InsertPlayer();
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsFullWindow)
                {
                    MainPage.Current.RemovePlayer();
                    page.InsertPlayer();
                }
            }
        }
        /// <summary>
        /// 进入小窗模式
        /// </summary>
        /// <param name="isCompact">是否为影院模式</param>
        public async void PlayVideoCompactOverlay(bool isCompact)
        {
            IPlayerPage page = GetCurrentPlayerPage();
            if (isCompact)
            {
                page.RemovePlayer();
                MainPage.Current.InsertPlayer();
                CurrentVideoPlayer.DanmakuBarVisibility = Visibility.Collapsed;
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsCompactOverlay)
                {
                    MainPage.Current.RemovePlayer();
                    page.InsertPlayer();
                    CurrentVideoPlayer.DanmakuBarVisibility = Visibility.Visible;
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                }
            }
        }
        /// <summary>
        /// 在新窗口中播放视频
        /// </summary>
        /// <param name="video">视频ID</param>
        /// <param name="cid">分片ID</param>
        public async void PlayVideoSeparate(VideoDetail video, int cid)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(MiniPlayerPage), new Tuple<VideoDetail, int>(video, cid));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            if (CurrentPageType == typeof(VideoPage))
                CurrentPagePanel.MainPageBack();
        }
        /// <summary>
        /// 在新窗口中播放视频
        /// </summary>
        /// <param name="bangumi">视频ID</param>
        /// <param name="cid">分片ID</param>
        public async void PlayVideoSeparate(BangumiDetail bangumi, Episode part)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(MiniPlayerPage), new Tuple<BangumiDetail, Episode>(bangumi, part));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            if (CurrentPageType == typeof(VideoPage))
                CurrentPagePanel.MainPageBack();
        }
        /// <summary>
        /// 显示网页弹出层
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="url">地址</param>
        public void ShowWebPopup(string title, string url)
        {
            if (_webPopup == null)
                _webPopup = new WebPopup();
            _webPopup.Init(title, url);
            _webPopup.ShowPopup();
        }
        /// <summary>
        /// 显示专栏文章弹出层
        /// </summary>
        /// <param name="title">标题</param>
        public void ShowDoucmentPopup(string title, int id)
        {
            if (_documentPopup == null)
                _documentPopup = new DocumentPopup();
            _documentPopup.Init(title, id);
            _documentPopup.ShowPopup();
        }
        /// <summary>
        /// 显示评论详情弹出层
        /// </summary>
        /// <param name="title">标题</param>
        public async void ShowReplyDetailPopup(string replyId, string oid, string type)
        {
            if (_replyDetailPopup == null)
                _replyDetailPopup = new ReplyDetailPopup();
            _replyDetailPopup.ShowPopup();
            await _replyDetailPopup.Init(replyId, oid, type);
        }
        public void FontInit()
        {
            var fonts = SystemFont.GetFonts();
            if (fonts != null && fonts.Count > 0)
                fonts.ForEach(p => FontCollection.Add(p));
        }
    }
}
