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

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class TrimTextBlock : UserControl
    {
        public TrimTextBlock()
        {
            this.InitializeComponent();
        }
        private bool _isShowTotal = false;
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TrimTextBlock), new PropertyMetadata("", new PropertyChangedCallback(Text_Changed)));

        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string text)
            {
                var instance = d as TrimTextBlock;
                instance.Reset();
                instance.DisplayBlock.Text = "";
                instance.DisplayBlock.Text = text;
            }
        }

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.Register("MaxLines", typeof(int), typeof(TrimTextBlock), new PropertyMetadata(3));



        private void Reset()
        {
            _isShowTotal = false;
            DisplayBlock.MaxLines = 3;
            ShowTotalButton.Content = "显示全部";
            ShowTotalButton.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_IsTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
        {
            if (DisplayBlock.IsTextTrimmed)
                ShowTotalButton.Visibility = Visibility.Visible;
        }

        private void ShowTotalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isShowTotal)
            {
                DisplayBlock.MaxLines = 3;
                ShowTotalButton.Content = "显示全部";
            }
            else
            {
                DisplayBlock.MaxLines = 999;
                ShowTotalButton.Content = "折叠内容";
            }
            _isShowTotal = !_isShowTotal;
        }
    }
}
