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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class LevelBlock : UserControl
    {
        public LevelBlock()
        {
            this.InitializeComponent();
        }

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(LevelBlock), new PropertyMetadata(0,new PropertyChangedCallback(Level_Changed)));

        private static void Level_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue is int level)
            {
                var instance = d as LevelBlock;
                instance.LevelImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Level/level_{level}.png"));
            }
        }
    }
}
