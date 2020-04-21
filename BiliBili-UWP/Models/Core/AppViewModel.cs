using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Layout;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Others;
using BiliBili_UWP.Pages.Main;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
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
        public WebPopup _webPopup;

        public List<Tuple<Guid, Action<Size>>> WindowsSizeChangedNotify { get; set; } = new List<Tuple<Guid, Action<Size>>>();
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
                //if (_updatePopup == null)
                //    _updatePopup = new UpdatePanel(updateInfo);
                //_updatePopup.ShowPopup();
                AppTool.WriteLocalSetting(Settings.LocalVersion, nowVersion);
            }
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="aid">AV号</param>
        /// <param name="sender">触发控件（用于查找封面以实现连接动画）</param>
        public void PlayVideo(int aid,object sender=null)
        {
            if (CurrentPagePanel.IsSubPageOpen)
                CurrentPagePanel.IsSubPageOpen = false;
            SelectedSideMenuItem = null;
            if (sender != null)
            {
                var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("VideoConnectedAnimation", image);
            }
            CurrentSidePanel.SetSelectedItem(Enums.SideMenuItemType.Line);
            CurrentPagePanel.NavigateToPage(Enums.SideMenuItemType.Player, aid);
        }
        /// <summary>
        /// 进入全屏模式
        /// </summary>
        /// <param name="isFull">是否为全屏模式</param>
        public void PlayVideoFullScreen(bool isFull)
        {
            if (isFull)
            {
                PlayerPage.Current.RemovePlayer();
                MainPage.Current.InsertPlayer();
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
                if (!CurrentVideoPlayer.MTC.IsCinema)
                {
                    MainPage.Current.RemovePlayer();
                    PlayerPage.Current.InsertPlayer();
                }
            }
        }
        /// <summary>
        /// 进入影院模式
        /// </summary>
        /// <param name="isCinema">是否为影院模式</param>
        public void PlayVideoCinema(bool isCinema)
        {
            if (isCinema)
            {
                PlayerPage.Current.RemovePlayer();
                MainPage.Current.InsertPlayer();
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsFullWindow)
                {
                    MainPage.Current.RemovePlayer();
                    PlayerPage.Current.InsertPlayer();
                }
            }
        }
        /// <summary>
        /// 进入小窗模式
        /// </summary>
        /// <param name="isCompact">是否为影院模式</param>
        public async void PlayVideoCompactOverlay(bool isCompact)
        {
            if (isCompact)
            {
                PlayerPage.Current.RemovePlayer();
                MainPage.Current.InsertPlayer();
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsCompactOverlay)
                {
                    MainPage.Current.RemovePlayer();
                    PlayerPage.Current.InsertPlayer();
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                }
            }
        }
        /// <summary>
        /// 在新窗口中播放视频
        /// </summary>
        /// <param name="video">视频ID</param>
        /// <param name="cid">分片ID</param>
        public async void PlayVideoSeparate(VideoDetail video,int cid)
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
            if (CurrentPageType == typeof(PlayerPage))
                CurrentPagePanel.MainPageBack();
        }
        /// <summary>
        /// 显示网页弹出层
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="url">地址</param>
        public void ShowWebPopup(string title,string url)
        {
            if (_webPopup == null)
                _webPopup = new WebPopup();
            _webPopup.Init(title, url);
            _webPopup.ShowPopup();
        }
    }
}
