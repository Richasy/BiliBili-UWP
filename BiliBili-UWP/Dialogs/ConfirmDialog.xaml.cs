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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Dialogs
{
    public sealed partial class ConfirmDialog : ContentDialog
    {
        public ConfirmDialog()
        {
            this.InitializeComponent();
            Title = "提醒";
            PrimaryButtonText = "确认";
            CloseButtonText = "取消";
        }

        public ConfirmDialog(string content) : this()
        {
            DisplayBlock.Text = content;
        }

        public ConfirmDialog(string title,string content) : this()
        {
            Title = title;
            DisplayBlock.Text = content;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
