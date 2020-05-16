using NSDanmaku.Helper;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace NSDanmaku.Controls
{
    public sealed partial class TantanDialog : ContentDialog
    {
        TanTanPlay tantan;
        public TantanDialog()
        {
            this.InitializeComponent();
            tantan = new TanTanPlay();
        }
        public event EventHandler<List<NSDanmaku.Model.DanmakuModel>> ReturnDanmakus;
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (list_Items.SelectedItem==null)
            {
                args.Cancel = true;
                return;
            }
            var data = list_Items.SelectedItem as Model.episodes;
            ReturnDanmakus(null,await tantan.GetDanmakus(data.episodeId));
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void txt_search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            txt_error.Visibility = Visibility.Collapsed;
            if (txt_search.Text.Length==0)
            {
                ShowError("请输入关键字");
                return;
            }
            try
            {
                list_Items.ItemsSource = await tantan.Search(txt_search.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            txt_error.Visibility = Visibility.Visible ;
            txt_error.Text = msg;
        }


    }
}
