using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class RenderTextBlock : UserControl
    {
        
        public RenderTextBlock()
        {
            this.InitializeComponent();
        }

        public void RenderText(string text)
        {
            text = text.Replace("\n", "\n\n");
            if (EmoteSource != null && EmoteSource.Count > 0)
            {
                foreach (var item in EmoteSource)
                {
                    double width = item.Value.meta.size * Convert.ToInt32(FontSize);
                    text = text.Replace(item.Key, $"<sub>!{item.Key}({item.Value.url} ={width})</sub>");
                }
            }
            else
            {
                foreach (var item in App.BiliViewModel._emojis)
                {
                    if (text.Contains(item.name))
                    {
                        double width = Convert.ToInt32(item.size) * Convert.ToInt32(FontSize);
                        text = text.Replace(item.name, $"<sub>!{item.name}({item.url} ={width})</sub>");
                    }
                }
            }
            richBlock.Text = text;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RenderTextBlock), new PropertyMetadata("", new PropertyChangedCallback(Text_Changed)));

        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is string v)
            {
                var instance = d as RenderTextBlock;
                instance.RenderText(v);
            }
        }

        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(RenderTextBlock), new PropertyMetadata(App.AppViewModel.BasicFontSize));

        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(RenderTextBlock), new PropertyMetadata(UIHelper.GetThemeBrush(Models.Enums.ColorType.NormalTextColor)));

        public int LineHeight
        {
            get { return (int)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(int), typeof(RenderTextBlock), new PropertyMetadata(Convert.ToInt32(Math.Floor(App.AppViewModel.BasicFontSize*1.3))));

        public Dictionary<string, Emote> EmoteSource
        {
            get { return (Dictionary<string, Emote>)GetValue(EmoteSourceProperty); }
            set { SetValue(EmoteSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmoteSourceProperty =
            DependencyProperty.Register("EmoteSource", typeof(Dictionary<string, Emote>), typeof(RenderTextBlock), new PropertyMetadata(null));



        private void richBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            App.AppViewModel.HandleUri(e.Link);
        }
    }
}
