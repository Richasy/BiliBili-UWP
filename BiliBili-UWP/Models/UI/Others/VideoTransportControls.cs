using NSDanmaku.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliBili_UWP.Models.UI.Others
{
    public class VideoTransportControls:MediaTransportControls
    {
        public Danmaku DanmakuControls = null;
        private AppBarButton _fullWindowButton;
        private AppBarButton _playButton;
        public bool IsInit = false;
        public VideoTransportControls()
        {
            this.DefaultStyleKey = typeof(VideoTransportControls);
        }
        public event EventHandler<Danmaku> DanmakuLoaded;
        public event EventHandler<bool> FullWindowChanged;
        public event EventHandler<bool> PlayButtonClick;
        public MediaPlayerElement MediaPlayerElement;
        protected override void OnApplyTemplate()
        {
            var _danmukuControls = GetTemplateChild("DanmakuControls") as Danmaku;
            DanmakuControls = _danmukuControls;
            DanmakuLoaded?.Invoke(this, DanmakuControls);

            _fullWindowButton = GetTemplateChild("CustomFullWindowButton") as AppBarButton;
            _fullWindowButton.Click += FullWindowButton_Click;
            _playButton = GetTemplateChild("CustomPlayPauseButton") as AppBarButton;
            _playButton.Click += PlayButton_Click;

            base.OnApplyTemplate();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        private void FullWindowButton_Click(object sender, RoutedEventArgs e)
        {
            IsFullWindow = !IsFullWindow;
        }

        private FontIcon GetIcon(string text)
        {
            return new FontIcon()
            {
                FontFamily = UIHelper.GetFontFamily("Icon"),
                Glyph = text
            };
        }

        public bool IsFullWindow
        {
            get { return (bool)GetValue(IsFullWindowProperty); }
            set { SetValue(IsFullWindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFullWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFullWindowProperty =
            DependencyProperty.Register("IsFullWindow", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false,new PropertyChangedCallback(IsFullWindow_Changed)));

        private static void IsFullWindow_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            instance.FullWindowChanged?.Invoke(instance, v);
            instance._fullWindowButton.Icon = v ? instance.GetIcon("") : instance.GetIcon("");
            instance.DanmakuControls.UpdateLayout();
        }

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false,new PropertyChangedCallback(IsPlaying_Changed)));

        private static void IsPlaying_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            instance._playButton.Icon = v ? instance.GetIcon("") : instance.GetIcon("");

            if (!instance.IsInit)
                return;
            if (instance.DanmakuControls != null)
            {
                if (v)
                    instance.DanmakuControls.ResumeDanmaku();
                else
                    instance.DanmakuControls.PauseDanmaku();
            }
            if (instance.MediaPlayerElement != null)
            {
                var player = instance.MediaPlayerElement.MediaPlayer;
                if (v)
                    player.Play();
                else if (player.PlaybackSession.CanPause)
                    player.Pause();
            }
            instance.PlayButtonClick?.Invoke(instance, v);
        }
    }
}
