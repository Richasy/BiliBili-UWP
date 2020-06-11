using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Service;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Share.Sub.Anime
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimelinePage : Page,IRefreshPage
    {
        private ObservableCollection<Timeline> TimelineCollection = new ObservableCollection<Timeline>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private int _type = 0;
        public TimelinePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "时间表";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if(e.Parameter!=null && e.Parameter is int type)
            {
                if (type != _type)
                {
                    _type = type;
                    await Refresh();
                }
            }
            base.OnNavigatedTo(e);
        }
        public async Task Refresh()
        {
            TimelineCollection.Clear();
            HolderText.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = true;
            var data = await _animeService.GetTimelineAsync(_type);
            if (data != null)
            {
                data.ForEach(p => TimelineCollection.Add(p));
            }
            HolderText.Visibility = TimelineCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingRing.IsActive = false;
            await Task.Delay(300);
            TimelineListView.ScrollIntoView(TimelineCollection.Where(p => p.is_today == 1).FirstOrDefault(),ScrollIntoViewAlignment.Leading);
        }

        private void Anime_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as TimelineEpisode;
            if (item.episode_id > 0)
                App.AppViewModel.PlayBangumi(item.episode_id, null, true);
            else
                App.AppViewModel.PlayBangumi(item.season_id);
        }
    }
}
