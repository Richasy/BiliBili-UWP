using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Dialogs
{
    public sealed partial class ShowImageDialog : ContentDialog
    {
        private string _source = "";
        private ObservableCollection<string> ImageCollection = new ObservableCollection<string>();
        public ShowImageDialog()
        {
            this.InitializeComponent();
        }
        public ShowImageDialog(string url):this()
        {
            _source = url;
            ImageCollection.Add(url);
        }
        public ShowImageDialog(ICollection<string> urls) : this()
        {
            _source = urls.First();
            foreach (var item in urls)
            {
                ImageCollection.Add(item);
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            PrimaryButtonText = "下载中";
            IsPrimaryButtonEnabled = false;
            try
            {
                var file = await IOTool.GetSaveFileAsync(".png", "保存的图片.png", "PNG 图片");
                if (file != null)
                {
                    var stream = await BiliTool.GetStreamFromWebAsync(_source);
                    using (var fileStream = await file.OpenStreamForWriteAsync())
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                    new TipPopup("下载完成").ShowMessage();
                }
            }
            catch (Exception)
            {
                new TipPopup("下载图片失败").ShowMessage();
            }
            PrimaryButtonText = "另存为";
            IsPrimaryButtonEnabled = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            SecondaryButtonText = "复制中";
            IsSecondaryButtonEnabled = false;
            var package = new DataPackage();
            package.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(_source)));
            Clipboard.SetContent(package);
            new TipPopup("复制完成").ShowMessage();
            SecondaryButtonText = "复制";
            IsSecondaryButtonEnabled = true;
        }
    }
}
