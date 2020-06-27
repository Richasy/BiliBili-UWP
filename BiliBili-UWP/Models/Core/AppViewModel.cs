using BiliBili_Controls.Extensions;
using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Layout;
using BiliBili_UWP.Components.Others;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using BiliBili_UWP.Models.UI.Others;
using BiliBili_UWP.Pages.Main;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Helpers;
using Richasy.Font.UWP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace BiliBili_UWP.Models.Core
{
    public class AppViewModel
    {
        public double StateChangeWidth = 1000;
        public AppMenuItem SelectedSideMenuItem { get; set; }
        public Type CurrentPageType { get; set; }
        public SidePanel CurrentSidePanel;
        public PagePanel CurrentPagePanel;
        public SubPageControl CurrentSubPageControl;
        public VideoPlayer CurrentVideoPlayer;
        public PlayerType CurrentPlayerType;
        public WebPopup _webPopup;
        public DocumentPopup _documentPopup;
        public ReplyDetailPopup _replyDetailPopup;
        public UpdatePopup _updatePopup;
        public DynamicDetailPopup _dynamicDetailPopup;
        public RepostPopup _repostPopup;
        public WebView _documentWebView;
        public bool IsInBackground;
        public string ConnectAnimationName = "";
        public double BasicFontSize = Convert.ToDouble(AppTool.GetLocalSetting(Settings.BasicFontSize, "14"));
        public TabletVideoDetailBlock CurrentVideoDetailBlock;
        public TabletBangumiDetailBlock CurrentBangumiDetailBlock;
        public TopPanel CurrentTopPanel;
        public bool IsVideoPageInit = false;
        public List<Tuple<Guid, Action<Size>>> WindowsSizeChangedNotify { get; set; } = new List<Tuple<Guid, Action<Size>>>();
        public ObservableCollection<SystemFont> FontCollection = new ObservableCollection<SystemFont>();
        public bool IsEnableAnimation = AppTool.GetBoolSetting(Settings.EnableAnimation);
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
        public async void AppInitByActivated(string argument)
        {
            QueryString args = QueryString.Parse(argument);
            args.TryGetValue("action", out string action);
            bool isPlay = CurrentVideoPlayer != null && (CurrentVideoPlayer.MTC.IsFullWindow || CurrentVideoPlayer.MTC.IsCinema || CurrentVideoPlayer.MTC.IsCompactOverlay);
            switch (action)
            {
                case "video":
                    if (isPlay)
                        return;
                    args.TryGetValue("aid", out string videoAid);
                    args.TryGetValue("bvid", out string videoBvId);
                    args.TryGetValue("cid", out string videoCid);
                    var videoArgs = new VideoActiveArgs();
                    if (!string.IsNullOrEmpty(videoAid))
                        videoArgs.aid = Convert.ToInt32(videoAid);
                    if (!string.IsNullOrEmpty(videoBvId))
                        videoArgs.bvid = videoBvId;
                    if (!string.IsNullOrEmpty(videoCid))
                        videoArgs.cid = Convert.ToInt32(videoCid);
                    PlayVideo(videoArgs);
                    break;
                case "bangumi":
                    if (isPlay)
                        return;
                    args.TryGetValue("epid", out string AnimeEpid);
                    PlayBangumi(Convert.ToInt32(AnimeEpid), isEp: true);
                    break;
                case "screenshot":
                    args.TryGetValue("image", out string screenShotName);
                    var picLib = await KnownFolders.PicturesLibrary.CreateFolderAsync("Bili ScreenShot", CreationCollisionOption.OpenIfExists);
                    var screenFile = await picLib.CreateFileAsync(screenShotName, CreationCollisionOption.OpenIfExists);
                    await Launcher.LaunchFileAsync(screenFile);
                    break;
                default:
                    break;
            }
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
            //App._watch.Start();
            CurrentSubPageControl.CheckSubReplyPage();
            SelectedSideMenuItem = null;
            if (sender != null && IsEnableAnimation && !IsVideoPageInit)
            {
                //var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                string animationName = "VideoConnectedAnimation" + Guid.NewGuid().ToString("N");
                ConnectAnimationName = animationName;
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(animationName, sender as UIElement);
            }
            else
                ConnectAnimationName = "";
            if (App._isTabletMode)
            {
                CurrentTopPanel.SetSelectedItem(AppMenuItemType.Line);
                TabletMainPage.Current.NavigateToPage(AppMenuItemType.VideoPlayer,aid);
            }
            else
            {
                CurrentSidePanel.SetSelectedItem(AppMenuItemType.Line);
                CurrentPagePanel.NavigateToPage(AppMenuItemType.VideoPlayer, new Tuple<int, string>(aid, fromSign));
            }
            
            //App._watch.Stop();
        }
        public void PlayVideo(VideoActiveArgs args)
        {
            SelectedSideMenuItem = null;
            CurrentSubPageControl.CheckSubReplyPage();
            if (App._isTabletMode)
            {
                CurrentTopPanel.SetSelectedItem(AppMenuItemType.Line);
                TabletMainPage.Current.NavigateToPage(AppMenuItemType.VideoPlayer, args);
            }
            else
            {
                CurrentSidePanel.SetSelectedItem(AppMenuItemType.Line);
                CurrentPagePanel.NavigateToPage(AppMenuItemType.VideoPlayer, args);
            }
        }
        /// <summary>
        /// 播放视频列表
        /// </summary>
        /// <param name="aid">AV号</param>
        /// <param name="videoList">播放列表</param>
        public void PlayVideoList(int aid, object sender, List<VideoDetail> videoList)
        {
            CurrentSubPageControl.CheckSubReplyPage();
            SelectedSideMenuItem = null;
            if (sender != null && IsEnableAnimation)
            {
                var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("VideoConnectedAnimation", image);
            }
            if (App._isTabletMode)
            {
                CurrentTopPanel.SetSelectedItem(AppMenuItemType.Line);
                TabletMainPage.Current.NavigateToPage(AppMenuItemType.VideoPlayer, new Tuple<int, List<VideoDetail>>(aid, videoList));
            }
            else
            {
                CurrentSidePanel.SetSelectedItem(AppMenuItemType.Line);
                CurrentPagePanel.NavigateToPage(AppMenuItemType.VideoPlayer, new Tuple<int, List<VideoDetail>>(aid, videoList));
            }
        }
        /// <summary>
        /// 播放番剧
        /// </summary>
        /// <param name="epid">AV号</param>
        /// <param name="sender">触发控件（用于查找封面以实现连接动画）</param>
        public void PlayBangumi(int epid, object sender = null, bool isEp = false)
        {
            CurrentSubPageControl.CheckSubReplyPage();
            SelectedSideMenuItem = null;
            if (sender != null && IsEnableAnimation)
            {
                var image = VisualTreeExtension.VisualTreeFindName<FrameworkElement>((FrameworkElement)sender, "VideoCover");
                if (image != null)
                {
                    string animationName = "BangumiConnectedAnimation" + Guid.NewGuid().ToString("N");
                    ConnectAnimationName = animationName;
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(animationName, image);
                }
                else
                    ConnectAnimationName = "";
            }
            if (App._isTabletMode)
            {
                CurrentTopPanel.SetSelectedItem(AppMenuItemType.Line);
                TabletMainPage.Current.NavigateToPage(AppMenuItemType.VideoPlayer, new Tuple<int, bool>(epid, isEp));
            }
            else
            {
                CurrentSidePanel.SetSelectedItem(AppMenuItemType.Line);
                if (isEp)
                    CurrentPagePanel.NavigateToPage(AppMenuItemType.BangumiPlayer, new Tuple<int, bool>(epid, isEp));
                else
                    CurrentPagePanel.NavigateToPage(AppMenuItemType.BangumiPlayer, epid);
            }
        }
        /// <summary>
        /// 获取当前的播放页
        /// </summary>
        /// <returns></returns>
        public IPlayerHost GetCurrentPlayerPage()
        {
            IPlayerHost page = null;
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
        /// 获取当前的播放容器
        /// </summary>
        /// <returns></returns>
        public IPlayerHost GetCurrentPlayerBlock()
        {
            IPlayerHost host = null;
            switch (CurrentPlayerType)
            {
                case PlayerType.Video:
                    host = CurrentVideoDetailBlock;
                    break;
                case PlayerType.Bangumi:
                    host = CurrentBangumiDetailBlock;
                    break;
                case PlayerType.Live:
                    break;
                default:
                    break;
            }
            return host;
        }
        /// <summary>
        /// 进入全屏模式
        /// </summary>
        /// <param name="isFull">是否为全屏模式</param>
        public void PlayVideoFullScreen(bool isFull)
        {
            IPlayerHost hostPage = App._isTabletMode ? TabletMainPage.Current as IPlayerHost : DesktopMainPage.Current as IPlayerHost;
            IPlayerHost host = App._isTabletMode ? GetCurrentPlayerBlock() : GetCurrentPlayerPage();
            if (isFull)
            {
                host.RemovePlayer();
                hostPage.InsertPlayer();
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
                if (!CurrentVideoPlayer.MTC.IsCinema)
                {
                    hostPage.RemovePlayer();
                    host.InsertPlayer();
                }
            }
        }
        /// <summary>
        /// 进入影院模式
        /// </summary>
        /// <param name="isCinema">是否为影院模式</param>
        public void PlayVideoCinema(bool isCinema)
        {
            IPlayerHost hostPage = App._isTabletMode ? TabletMainPage.Current as IPlayerHost : DesktopMainPage.Current as IPlayerHost;
            IPlayerHost host = App._isTabletMode ? GetCurrentPlayerBlock() : GetCurrentPlayerPage();
            if (isCinema)
            {
                host.RemovePlayer();
                hostPage.InsertPlayer();
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsFullWindow)
                {
                    hostPage.RemovePlayer();
                    host.InsertPlayer();
                }
            }
        }
        /// <summary>
        /// 进入小窗模式
        /// </summary>
        /// <param name="isCompact">是否为影院模式</param>
        public async void PlayVideoCompactOverlay(bool isCompact)
        {
            IPlayerHost hostPage = App._isTabletMode ? TabletMainPage.Current as IPlayerHost : DesktopMainPage.Current as IPlayerHost;
            IPlayerHost host = App._isTabletMode ? GetCurrentPlayerBlock() : GetCurrentPlayerPage();
            if (isCompact)
            {
                host.RemovePlayer();
                hostPage.InsertPlayer();
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
            else
            {
                if (!CurrentVideoPlayer.MTC.IsCompactOverlay)
                {
                    hostPage.RemovePlayer();
                    host.InsertPlayer();
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
        public async void PlayVideoSeparate(VideoDetail video, int cid, bool isCloseCurrentPage = true)
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
            if (!App._isTabletMode && CurrentPageType == typeof(VideoPage) && isCloseCurrentPage)
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
            if (!App._isTabletMode && CurrentPageType == typeof(BangumiPage))
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
            var fonts = SystemFont.GetSystemFonts("zh-cn");
            if (fonts != null && fonts.Count > 0)
                fonts.ForEach(p => FontCollection.Add(p));
        }

        public void CheckPlayerOnBackgroundChanged()
        {
            bool isStopInBackground = AppTool.GetBoolSetting(Settings.IsStopInBackground, false);
            if (IsInBackground)
            {
                if (isStopInBackground && CurrentVideoPlayer != null)
                    CurrentVideoPlayer.MTC.IsPlaying = false;
            }
            else
            {
                if (isStopInBackground && CurrentVideoPlayer != null)
                    CurrentVideoPlayer.MTC.IsPlaying = true;
            }
        }

        /// <summary>
        /// 注册后台任务
        /// </summary>
        /// <param name="type">注册类型</param>
        /// <returns></returns>
        public async Task<bool> RegisterBackgroundTask(string type)
        {
            string backgroundTaskName = $"{type}NotificationTask";

            if (BackgroundTaskHelper.IsBackgroundTaskRegistered(backgroundTaskName))
            {
                return true;
            }
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status.ToString().Contains("Allowed"))
            {
                BackgroundTaskHelper.Register(backgroundTaskName, $"BiliBili_Notification.{type}Notification", new TimeTrigger(15, false), false, true, new SystemCondition(SystemConditionType.InternetAvailable));
                return true;
            }
            else
            {
                new TipPopup("需要开启后台通知").ShowError();
                return false;
            }
        }
        /// <summary>
        /// 注销后台任务
        /// </summary>
        /// <param name="type">类型</param>
        public void UnRegisterBackgroundTask(string type)
        {
            string backgroundTaskName = $"{type}NotificationTask";
            if (BackgroundTaskHelper.IsBackgroundTaskRegistered(backgroundTaskName))
                BackgroundTaskHelper.Unregister(backgroundTaskName);
        }
        /// <summary>
        /// 显示动态详情弹出层（仅限图片、专栏和网页）
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="dynamic">动态信息</param>
        /// <param name="data">数据</param>
        /// <param name="rid">回复ID</param>
        public void ShowDynamicDetailPopup(SlimUserInfo user, string dynamic, object data, string rid)
        {
            if (_dynamicDetailPopup == null)
                _dynamicDetailPopup = new DynamicDetailPopup();
            _dynamicDetailPopup.ShowPopup();
            _dynamicDetailPopup.User = user;
            _dynamicDetailPopup.Data = data;
            _dynamicDetailPopup.Dynamic = dynamic;
            _dynamicDetailPopup.InitReply(rid);
        }
        public void ShowRepostPopup(string origin, Topic topic)
        {
            if (_repostPopup == null)
                _repostPopup = new RepostPopup();
            _repostPopup.ShowPopup();
            _repostPopup.Init(origin, topic);
        }
        public void ShowRepostPopup(string origin, VideoDetail video)
        {
            if (_repostPopup == null)
                _repostPopup = new RepostPopup();
            _repostPopup.ShowPopup();
            _repostPopup.Init(origin, video);
        }
        public void ShowRepostPopup(string origin, BangumiDetail bangumi, Episode part)
        {
            if (_repostPopup == null)
                _repostPopup = new RepostPopup();
            _repostPopup.ShowPopup();
            _repostPopup.Init(origin, bangumi, part);
        }
        public void HandleUri(string url, string title = "")
        {
            var result = BiliTool.GetResultFromUri(url);
            if (result.Type.ToString().Contains("Video"))
            {
                if (result.Type == UriType.VideoA)
                    PlayVideo(Convert.ToInt32(result.Param));
                else
                {
                    var args = new VideoActiveArgs() { bvid = result.Param };
                    PlayVideo(args);
                }
            }
            else if (result.Type == UriType.Bangumi)
            {
                PlayBangumi(Convert.ToInt32(result.Param), null, true);
            }
            else if (result.Type == UriType.Document)
            {
                ShowDoucmentPopup(title, Convert.ToInt32(result.Param));
            }
            else if (result.Type == UriType.Web)
            {
                ShowWebPopup(title, result.Param);
            }
        }

        public void NavigateToSubPage(Type page, object parameter = null, bool isBack = false)
        {
            CurrentSubPageControl.NavigateToSubPage(page, parameter, isBack);
            if (App._isTabletMode)
                TabletMainPage.Current.IsSubPageOpen = true;
            else
                CurrentPagePanel.IsSubPageOpen = true;
        }
        public async void AccelertorKeyActivedHandle(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var esc = Window.Current.CoreWindow.GetKeyState(VirtualKey.Escape);
                var space = Window.Current.CoreWindow.GetKeyState(VirtualKey.Space);
                var f11 = Window.Current.CoreWindow.GetKeyState(VirtualKey.F11);
                var f10 = Window.Current.CoreWindow.GetKeyState(VirtualKey.F10);
                var f2 = Window.Current.CoreWindow.GetKeyState(VirtualKey.F2);
                var left = Window.Current.CoreWindow.GetKeyState(VirtualKey.Left);
                var right = Window.Current.CoreWindow.GetKeyState(VirtualKey.Right);
                var up = Window.Current.CoreWindow.GetKeyState(VirtualKey.Up);
                var down = Window.Current.CoreWindow.GetKeyState(VirtualKey.Down);
                var shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
                var player = CurrentVideoPlayer;

                if (esc.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (App.AppViewModel._dynamicDetailPopup != null && App.AppViewModel._dynamicDetailPopup._popup.IsOpen)
                    {
                        App.AppViewModel._dynamicDetailPopup.HidePopup();
                        return;
                    }
                    if (player != null)
                    {
                        if (player.MTC.IsFullWindow)
                        {
                            args.Handled = true;
                            player.MTC.IsFullWindow = false;
                        }
                        else if (player.MTC.IsCinema)
                        {
                            args.Handled = true;
                            player.MTC.IsCinema = false;
                        }
                        player.Focus(FocusState.Programmatic);
                    }
                }
                else if (space.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus && (player.MTC.IsFullWindow || player.MTC.IsCinema))
                    {
                        args.Handled = true;
                        player.MTC.IsPlaying = !player.MTC.IsPlaying;
                        player.Focus(FocusState.Programmatic);
                    }
                }
                else if (f11.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.MTC.IsFullWindow = !player.MTC.IsFullWindow;
                    }
                }
                else if (f10.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.MTC.IsCompactOverlay = !player.MTC.IsCompactOverlay;
                    }
                }
                else if (f2.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        await player.ChangeDanmakuStatus();
                    }
                }
                else if (left.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.SkipRewind();
                    }
                }
                else if (right.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.SkipForward();
                    }
                }
                else if (up.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.UpVolume();
                    }
                }
                else if (down.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.DownVolume();
                    }
                }
                else if (shift.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (args.VirtualKey == VirtualKey.S)
                    {
                        //截图
                        if (player != null && player.IsFocus)
                        {
                            await player.ScreenShot();
                        }
                    }
                }
            }
        }
    }
}
