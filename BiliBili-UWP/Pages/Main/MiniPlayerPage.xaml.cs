using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MiniPlayerPage : Page
    {
        public MiniPlayerPage()
        {
            CustomXamlResourceLoader.Current = new CustomResourceLoader();
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is Tuple<VideoDetail, int> data)
                    await VideoPlayer.Init(data.Item1, data.Item2);
                else if (e.Parameter is Tuple<BangumiDetail, Episode> bangu)
                    await VideoPlayer.Init(bangu.Item1, bangu.Item2);
                bool isShowDanmakuBar = AppTool.GetBoolSetting(Settings.IsShowDanmakuBarInSeparate, false);
                VideoPlayer.DanmakuBarVisibility = isShowDanmakuBar ? Visibility.Visible : Visibility.Collapsed;
                VideoPlayer.ChangeDanmakuBarDisplayMode(false, isShowDanmakuBar);
                UIHelper.SetTitleBarColor();
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            VideoPlayer.Close();
            base.OnNavigatingFrom(e);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().Consolidated -= Page_Consolidated;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().Consolidated += Page_Consolidated;
        }

        private void Page_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            VideoPlayer.Close();
            Window.Current.Close();
        }

        private void VideoPlayer_MTCLoaded(object sender, EventArgs e)
        {
            VideoPlayer.CompactOverlayButtonVisibility = Visibility.Collapsed;
            VideoPlayer.FullWindowButtonVisibility = Visibility.Collapsed;
            VideoPlayer.CinemaButtonVisibility = Visibility.Collapsed;
            VideoPlayer.SeparateButtonVisibility = Visibility.Collapsed;
            VideoPlayer.ResetPlayRate();
        }
    }
}
