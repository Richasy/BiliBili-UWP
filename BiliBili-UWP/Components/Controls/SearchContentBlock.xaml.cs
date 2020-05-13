using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Components.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class SearchContentBlock : UserControl
    {
        public SearchContentBlock()
        {
            this.InitializeComponent();
        }
        private string _cardType = "";

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(SearchContentBlock), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var data = e.NewValue;
                var instance = d as SearchContentBlock;
                instance.MainContentControl.Content = data;
                if (data is SearchVideo)
                {
                    instance._cardType = "video";
                    instance.MainContentControl.ContentTemplate = instance.VideoTemplate;
                }
                else if(data is SearchAnime anime)
                {
                    if (anime.season_type_name == "番剧")
                        instance._cardType = "anime";
                    else
                        instance._cardType = "movie";
                    instance.MainContentControl.ContentTemplate = instance.AnimeTemplate;
                }
                else if(data is SearchUser)
                {
                    instance._cardType = "user";
                    instance.MainContentControl.ContentTemplate = instance.UserTemplate;
                }
                else if(data is SearchDocument)
                {
                    instance._cardType = "document";
                    var doc = data as SearchDocument;
                    if(string.IsNullOrEmpty(doc.cover) && doc.image_urls.Count > 0)
                    {
                        doc.cover = doc.image_urls.First();
                    }
                    instance.MainContentControl.ContentTemplate = instance.DocumentTemplate;
                }
            }
        }

        private async void FollowUserButton_Click(object sender, RoutedEventArgs e)
        {
            var user = Data as SearchUser;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                var btn = sender as AsyncButton;
                btn.IsLoading = true;
                bool result = await App.BiliViewModel._client.Account.FollowUser(user.mid);
                if (result)
                {
                    user.attentions = 1;
                    btn.Visibility = Visibility.Collapsed;
                    new TipPopup("已关注用户").ShowMessage();
                }
                else
                {
                    new TipPopup("关注失败").ShowError();
                }
                btn.IsLoading = false;
            }
        }

        private void WatchAnimeButton_Click(object sender, RoutedEventArgs e)
        {
            var d = Data as SearchAnime;
            App.AppViewModel.PlayBangumi(d.season_id);
        }

        private async void FollowAnimeButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AsyncButton;
            var anime = btn.DataContext as SearchAnime;
            var _animeService = App.BiliViewModel._client.Anime;
            bool result = false;
            btn.IsLoading = true;
            result = await _animeService.FollowBangumiAsync(anime.season_id);
            if (result)
            {
                anime.is_atten = 1;
                btn.Visibility = Visibility.Collapsed;
                new TipPopup(anime.render_follow).ShowMessage();
            }
            else
                new TipPopup("操作失败").ShowError();
        }
    }
}
