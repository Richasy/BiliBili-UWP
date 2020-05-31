using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using NSDanmaku.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BiliBili_UWP.Models.UI.Others
{
    public class VideoTransportControls : MediaTransportControls
    {
        public Danmaku DanmakuControls = null;
        private CommandBar _commandBar;
        private AppBarButton _fullWindowButton;
        private AppBarButton _playButton;
        private AppBarButton _compactOverlayButton;
        private AppBarButton _cinemaButton;
        private AppBarButton _separateButton;
        private AppBarButton _previousButton;
        private AppBarButton _nextButton;
        public AppBarComboBox _qualityComboBox;
        public AppBarComboBox _playRateComboBox;
        public ListView _subtitleListView;
        public bool IsInit = false;

        public VideoTransportControls()
        {
            this.DefaultStyleKey = typeof(VideoTransportControls);
        }
        public event EventHandler<Danmaku> DanmakuLoaded;
        public event EventHandler<bool> FullWindowChanged;
        public event EventHandler<bool> CinemaChanged;
        public event EventHandler<bool> PlayButtonClick;
        public event EventHandler<bool> CompactOverlayButtonClick;
        public event EventHandler<int> QualityChanged;
        public event EventHandler<double> PlayRateChanged;
        public event RoutedEventHandler PreviousButtonClick;
        public event RoutedEventHandler NextButtonClick;
        public event EventHandler<SubtitleIndexItem> SubtitleChanged;
        public event RoutedEventHandler SeparateButtonClick;
        public MediaPlayerElement MediaPlayerElement;
        protected override void OnApplyTemplate()
        {
            var _danmukuControls = GetTemplateChild("DanmakuControls") as Danmaku;
            DanmakuControls = _danmukuControls;
            DanmakuLoaded?.Invoke(this, DanmakuControls);

            _commandBar = GetTemplateChild("MediaControlsCommandBar") as CommandBar;
            _fullWindowButton = GetTemplateChild("CustomFullWindowButton") as AppBarButton;
            _fullWindowButton.Click += FullWindowButton_Click;
            _playButton = GetTemplateChild("CustomPlayPauseButton") as AppBarButton;
            _playButton.Click += PlayButton_Click;
            _compactOverlayButton = GetTemplateChild("CustomCompactOverlayButton") as AppBarButton;
            _compactOverlayButton.Click += CompactButtonClick;
            _cinemaButton = GetTemplateChild("CinemaButton") as AppBarButton;
            _cinemaButton.Click += CinemaButtonClick;
            _separateButton = GetTemplateChild("SeparateButton") as AppBarButton;
            _separateButton.Click += SeparateButtonClick;
            _previousButton = GetTemplateChild("CustomPreviousButton") as AppBarButton;
            _previousButton.Click += PreviousButton_Click;
            _nextButton = GetTemplateChild("CustomNextButton") as AppBarButton;
            _nextButton.Click += NextButton_Click;

            _qualityComboBox = GetTemplateChild("QualityComboBox") as AppBarComboBox;
            _qualityComboBox.SelectionChanged += QualityComboBox_SelectionChanged;
            _playRateComboBox = GetTemplateChild("PlayRateComboBox") as AppBarComboBox;
            _playRateComboBox.SelectionChanged += PlayRateComboBox_SelectionChanged;
            _subtitleListView = GetTemplateChild("SubtitleListView") as ListView;
            _subtitleListView.ItemClick += SubtitleListView_ItemClick;

            base.OnApplyTemplate();
        }

        private void PlayRateComboBox_SelectionChanged(object sender, object e)
        {
            var item = e as Tuple<double, string>;
            if (item != null)
                PlayRateChanged?.Invoke(this, item.Item1);
        }

        private void QualityComboBox_SelectionChanged(object sender, object e)
        {
            var item = e as Tuple<int, string>;
            if (item != null)
                QualityChanged?.Invoke(this, item.Item1);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousButtonClick?.Invoke(this, e);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NextButtonClick?.Invoke(this, e);
        }

        private void SubtitleListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SubtitleIndexItem;
            SubtitleChanged?.Invoke(this, item);
        }

        private void CinemaButtonClick(object sender, RoutedEventArgs e)
        {
            IsCinema = !IsCinema;
        }

        private void CompactButtonClick(object sender, RoutedEventArgs e)
        {
            IsCompactOverlay = !IsCompactOverlay;
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
            DependencyProperty.Register("IsFullWindow", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false, new PropertyChangedCallback(IsFullWindow_Changed)));

        private static void IsFullWindow_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            if (instance.IsCinema && v)
                instance.IsCinema = false;
            else if (instance.IsCompactOverlay && v)
                instance.IsCompactOverlay = false;
            instance.FullWindowChanged?.Invoke(instance, v);
            if (instance._fullWindowButton != null)
            {
                instance._fullWindowButton.Icon = v ? instance.GetIcon("") : instance.GetIcon("");
                instance.DanmakuControls.UpdateLayout();
            }
        }

        public bool IsCinema
        {
            get { return (bool)GetValue(IsCinemaProperty); }
            set { SetValue(IsCinemaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCinema.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCinemaProperty =
            DependencyProperty.Register("IsCinema", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false,new PropertyChangedCallback(IsCinema_Changed)));

        private static void IsCinema_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            if(instance.IsFullWindow && v)
                instance.IsFullWindow = false;
            else if (instance.IsCompactOverlay && v)
                instance.IsCompactOverlay = false;
            instance.CinemaChanged?.Invoke(instance, v);
            if (instance._cinemaButton != null)
            {
                instance._cinemaButton.Icon = v ? instance.GetIcon("") : instance.GetIcon("");
                instance.DanmakuControls.UpdateLayout();
            }
        }

        public Visibility CinemaButtonVisibility
        {
            get { return (Visibility)GetValue(CinemaButtonVisibilityProperty); }
            set { SetValue(CinemaButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CinemaButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CinemaButtonVisibilityProperty =
            DependencyProperty.Register("CinemaButtonVisibility", typeof(Visibility), typeof(VideoTransportControls), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(CinemaButtonVisibility_Changed)));

        private static void CinemaButtonVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            if (instance._commandBar == null)
                return;
            if ((Visibility)e.NewValue == Visibility.Collapsed)
                instance._commandBar.PrimaryCommands.Remove(instance._cinemaButton);
            else
                instance._commandBar.PrimaryCommands.Add(instance._cinemaButton);
        }

        public Visibility FullWindowButtonVisibility
        {
            get { return (Visibility)GetValue(FullWindowButtonVisibilityProperty); }
            set { SetValue(FullWindowButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullWindowButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullWindowButtonVisibilityProperty =
            DependencyProperty.Register("FullWindowButtonVisibility", typeof(Visibility), typeof(VideoTransportControls), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(FullWindowButtonVisibility_Changed)));

        private static void FullWindowButtonVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            if (instance._commandBar == null)
                return;
            if ((Visibility)e.NewValue == Visibility.Collapsed)
                instance._commandBar.PrimaryCommands.Remove(instance._fullWindowButton);
            else
                instance._commandBar.PrimaryCommands.Add(instance._fullWindowButton);
        }

        public Visibility CompactOverlayButtonVisibility
        {
            get { return (Visibility)GetValue(CompactOverlayButtonVisibilityProperty); }
            set { SetValue(CompactOverlayButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompactOverlayButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompactOverlayButtonVisibilityProperty =
            DependencyProperty.Register("CompactOverlayButtonVisibility", typeof(Visibility), typeof(VideoTransportControls), new PropertyMetadata(Visibility.Visible,new PropertyChangedCallback(CompactOverlayButtonVisibility_Changed)));

        private static void CompactOverlayButtonVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            if (instance._commandBar == null)
                return;
            if ((Visibility)e.NewValue == Visibility.Collapsed)
                instance._commandBar.PrimaryCommands.Remove(instance._compactOverlayButton);
            else
                instance._commandBar.PrimaryCommands.Add(instance._compactOverlayButton);
        }

        public Visibility SeparateButtonVisibility
        {
            get { return (Visibility)GetValue(SeparateButtonVisibilityProperty); }
            set { SetValue(SeparateButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeparateButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparateButtonVisibilityProperty =
            DependencyProperty.Register("SeparateButtonVisibility", typeof(Visibility), typeof(VideoTransportControls), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(SeparateButtonVisibility_Changed)));

        private static void SeparateButtonVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            if (instance._commandBar == null)
                return;
            if ((Visibility)e.NewValue == Visibility.Collapsed)
                instance._commandBar.PrimaryCommands.Remove(instance._separateButton);
            else
                instance._commandBar.PrimaryCommands.Add(instance._separateButton);
        }

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false, new PropertyChangedCallback(IsPlaying_Changed)));

        private static void IsPlaying_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            if (instance._playButton == null)
                return;
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

        public bool IsCompactOverlay
        {
            get { return (bool)GetValue(IsCompactOverlayProperty); }
            set { SetValue(IsCompactOverlayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCompactOverlay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCompactOverlayProperty =
            DependencyProperty.Register("IsCompactOverlay", typeof(bool), typeof(VideoTransportControls), new PropertyMetadata(false,new PropertyChangedCallback(IsCompactOverlay_Changed)));

        private static void IsCompactOverlay_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoTransportControls;
            var v = (bool)e.NewValue;
            if (!instance.IsInit)
                return;
            if (instance.IsFullWindow && v)
                instance.IsFullWindow = false;
            else if (instance.IsCinema && v)
                instance.IsCinema = false;
            if (instance.DanmakuControls != null)
            {
                bool isShow = AppTool.GetBoolSetting(BiliBili_Lib.Enums.Settings.IsShowDanmakuInCompactOverlay, false);
                if (!isShow && v)
                    instance.DanmakuControls.Visibility = Visibility.Collapsed;
                else
                    instance.DanmakuControls.Visibility = Visibility.Visible;
            }
            instance.CompactOverlayButtonClick?.Invoke(instance, v);
            instance.DanmakuControls.UpdateLayout();
        }

        public object QualityItemsSource
        {
            get { return (object)GetValue(QualityItemsSourceProperty); }
            set { SetValue(QualityItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QualityItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QualityItemsSourceProperty =
            DependencyProperty.Register("QualityItemsSource", typeof(object), typeof(VideoTransportControls), new PropertyMetadata(null));

        public object PlayRateItemsSource
        {
            get { return (object)GetValue(PlayRateItemsSourceProperty); }
            set { SetValue(PlayRateItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayRateItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayRateItemsSourceProperty =
            DependencyProperty.Register("PlayRateItemsSource", typeof(object), typeof(VideoTransportControls), new PropertyMetadata(null));



        public int QualitySelectIndex
        {
            get { return (int)GetValue(QualitySelectIndexProperty); }
            set { SetValue(QualitySelectIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QualitySelectIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QualitySelectIndexProperty =
            DependencyProperty.Register("QualitySelectIndex", typeof(int), typeof(VideoTransportControls), new PropertyMetadata(-1));

        public object SubtitleItemsSource
        {
            get { return (object)GetValue(SubtitleItemsSourceProperty); }
            set { SetValue(SubtitleItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleItemsSourceProperty =
            DependencyProperty.Register("SubtitleItemsSource", typeof(object), typeof(VideoTransportControls), new PropertyMetadata(null));

        public int SubtitleSelectIndex
        {
            get { return (int)GetValue(SubtitleSelectIndexProperty); }
            set { SetValue(SubtitleSelectIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleSelectIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleSelectIndexProperty =
            DependencyProperty.Register("SubtitleSelectIndex", typeof(int), typeof(VideoTransportControls), new PropertyMetadata(-1));

        public Visibility SubtitleHolderVisibility
        {
            get { return (Visibility)GetValue(SubtitleHolderVisibilityProperty); }
            set { SetValue(SubtitleHolderVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubtitleHolderVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubtitleHolderVisibilityProperty =
            DependencyProperty.Register("SubtitleHolderVisibility", typeof(Visibility), typeof(VideoTransportControls), new PropertyMetadata(Visibility.Collapsed));


    }
}
