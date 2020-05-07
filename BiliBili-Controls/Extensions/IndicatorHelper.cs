using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace BiliBili_Controls.Extensions
{
    public static class IndicatorService
    {
        #region Const Values

        private static readonly Vector2 c_frame1point1 = new Vector2(0.9f, 0.1f);
        private static readonly Vector2 c_frame1point2 = new Vector2(0.7f, 0.4f);
        private static readonly Vector2 c_frame2point1 = new Vector2(0.1f, 0.9f);
        private static readonly Vector2 c_frame2point2 = new Vector2(0.2f, 1f);

        #endregion Const Values

        #region Attach Properties

        public static string GetName(DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        public static bool GetIsScaleEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsScaleEnabledProperty);
        }

        public static void SetIsScaleEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScaleEnabledProperty, value);
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(IndicatorService), new PropertyMetadata(null, NamePropertyChanged));

        public static readonly DependencyProperty IsScaleEnabledProperty =
            DependencyProperty.RegisterAttached("IsScaleEnabled", typeof(bool), typeof(IndicatorService), new PropertyMetadata(true));

        private static void NamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string Name)
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    if (d is Selector selector)
                    {
                        selector.SelectionChanged -= OnSelectionChanged;
                        selector.SelectionChanged += OnSelectionChanged;
                    }
                    else if (d is Pivot pivot)
                    {
                        if (IsLoaded(pivot))
                        {
                            TryStartPivotHeaderAnimation(pivot, pivot.SelectedItem, null);
                        }
                        else
                        {
                            pivot.Loaded += _Loaded;
                        }
                        pivot.SelectionChanged -= OnSelectionChanged;
                        pivot.SelectionChanged += OnSelectionChanged;
                    }
                }
                else
                {
                    if (d is Selector selector)
                    {
                        selector.SelectionChanged -= OnSelectionChanged;
                    }
                    else if (d is Pivot pivot)
                    {
                        pivot.SelectionChanged -= OnSelectionChanged;
                    }
                }
            }
        }

        #endregion Attach Properties

        #region Event Methods

        private static void _Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Pivot pivot)
            {
                pivot.Loaded -= _Loaded;
                TryStartPivotHeaderAnimation(pivot, pivot.SelectedItem, null);
            }
            if (sender is Selector selector)
            {
                selector.Loaded -= _Loaded;
                TryStartSelectorAnimation(selector, selector.SelectedItem, null);

            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1 || e.RemovedItems.Count != 1) return;
            if (sender is Selector selector)
            {
                if (IsLoaded(selector))
                    TryStartSelectorAnimation(selector, e.AddedItems.FirstOrDefault(), e.RemovedItems.FirstOrDefault());
            }
            if (sender is Pivot pivot)
            {
                TryStartPivotHeaderAnimation(pivot, e.AddedItems.FirstOrDefault(), e.RemovedItems.FirstOrDefault());
            }
        }

        #endregion Event Methods

        #region Animation Methods

        private static void ResetCompositionValue(FrameworkElement element)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.StopAnimation("Translation.XY");
            visual.StopAnimation("Scale");
            visual.StopAnimation("CenterPoint");

            visual.Properties.InsertVector3("Translation", Vector3.Zero);
            visual.Scale = Vector3.One;
            visual.CenterPoint = Vector3.Zero;
        }

        private static void TryStartSelectorAnimation(Selector selector, object NewItem, object OldItem)
        {
            if (NewItem == null || OldItem == null) return;
            if (selector is Windows.UI.Xaml.Controls.ListViewBase listView)
            {
                if (listView.SelectionMode == ListViewSelectionMode.None || listView.SelectionMode == ListViewSelectionMode.Multiple) return;
            }
            else if (selector is ListBox listBox)
            {
                if (listBox.SelectionMode == SelectionMode.Multiple) return;
            }

            var name = GetName(selector);

            if (string.IsNullOrEmpty(name)) return;

            var oldContainer = selector.ContainerFromItem(OldItem) as FrameworkElement;
            var newContainer = selector.ContainerFromItem(NewItem) as FrameworkElement;

            var oldIndicator = oldContainer?.VisualTreeFindName(name);
            var newIndicator = newContainer?.VisualTreeFindName(name);

            if (newIndicator != null)
            {
                if (oldIndicator != null)
                {
                    var token = selector.GetHashCode().ToString();
                    TryStartAnimation(newIndicator, oldIndicator, GetIsScaleEnabled(selector));
                }
            }
        }

        private static void TryStartPivotHeaderAnimation(Pivot pivot, object NewItem, object OldItem)
        {
            if (NewItem == null) return;

            var NewIndex = -1;
            var OldIndex = -1;

            if (pivot.ContainerFromItem(OldItem) is PivotItem oldContainer)
            {
                OldIndex = pivot.IndexFromContainer(oldContainer);
            }

            if (pivot.ContainerFromItem(NewItem) is PivotItem newContainer)
            {
                NewIndex = pivot.IndexFromContainer(newContainer);
            }

            if (NewIndex >= 0)
            {
                var name = GetName(pivot);

                var Headers = pivot.VisualTreeFindAll<PivotHeaderPanel>();
                var Header = Headers.FirstOrDefault(c => c.Name == "Header");
                var StaticHeader = Headers.FirstOrDefault(c => c.Name == "StaticHeader");

                var newIndicator = GetPivotHeaderIndicator(Header, name, NewIndex);
                var newStaticIndicator = GetPivotHeaderIndicator(StaticHeader, name, NewIndex);

                if (OldIndex >= 0)
                {
                    var oldIndicator = GetPivotHeaderIndicator(Header, name, OldIndex);
                    var oldStaticIndicator = GetPivotHeaderIndicator(StaticHeader, name, OldIndex);

                    if (Header != null && newIndicator != null && oldIndicator != null)
                    {
                        TryStartPivotHeaderAnimation(Header, newIndicator, oldIndicator, GetIsScaleEnabled(pivot));
                    }
                    if (StaticHeader != null && newStaticIndicator != null && oldStaticIndicator != null)
                    {
                        TryStartPivotHeaderAnimation(StaticHeader, newStaticIndicator, oldStaticIndicator, GetIsScaleEnabled(pivot));
                    }
                }
                else
                {
                    if (newIndicator != null)
                    {
                        newIndicator.Visibility = Visibility.Visible;
                    }
                    if (newStaticIndicator != null)
                    {
                        newStaticIndicator.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private static void TryStartPivotHeaderAnimation(PivotHeaderPanel panel, FrameworkElement NewIndicator, FrameworkElement OldIndicator, bool isScaleEnabled)
        {
            NewIndicator.Visibility = Visibility.Visible;
            NewIndicator.UpdateLayout();
            if (IsLoaded(NewIndicator))
            {
                TryStartAnimation(NewIndicator, OldIndicator, isScaleEnabled);

                OldIndicator.Visibility = Visibility.Collapsed;
            }
            else
            {
                NewIndicator.Loaded += NewIndicatorLoaded;
            }

            void NewIndicatorLoaded(object sender, RoutedEventArgs args)
            {
                NewIndicator.Loaded -= NewIndicatorLoaded;
                TryStartAnimation(NewIndicator, OldIndicator, isScaleEnabled);
                NewIndicator.Visibility = Visibility.Visible;
                OldIndicator.Visibility = Visibility.Collapsed;
            }
        }


        private static void TryStartAnimation(FrameworkElement newIndicator, FrameworkElement oldIndicator, bool isScaleEnabled)
        {
            ResetCompositionValue(oldIndicator);
            if (isScaleEnabled)
            {
                TryStartAnimationWithScale(newIndicator, oldIndicator);
            }
            else
            {
                TryStartAnimation(newIndicator, oldIndicator);
            }
        }


        private static void TryStartAnimation(FrameworkElement newIndicator, FrameworkElement oldIndicator)
        {
            var compositor = Window.Current.Compositor;

            var oldSize = new Vector2((float)oldIndicator.ActualWidth, (float)oldIndicator.ActualHeight);
            var newSize = new Vector2((float)newIndicator.ActualWidth, (float)newIndicator.ActualHeight);

            var oldScale = oldSize / newSize;

            var oldOffset = newIndicator.TransformToVisual(oldIndicator).TransformPoint(new Windows.Foundation.Point(0, 0)).ToVector2();

            float startx = 0, starty = 0;

            if (oldOffset.X < 0)
            {
                startx = newSize.X;
                oldOffset.X = oldOffset.X + newSize.X - oldSize.X;
            }

            if (oldOffset.Y < 0)
            {
                starty = newSize.Y;
                oldOffset.Y = oldOffset.Y + newSize.Y - oldSize.Y;
            }

            var old_target = ElementCompositionPreview.GetElementVisual(oldIndicator);
            var new_target = ElementCompositionPreview.GetElementVisual(newIndicator);
            new_target.IsVisible = true;
            var duration = TimeSpan.FromSeconds(0.3d);
            var delay = TimeSpan.FromSeconds(0.01);

            var standard = compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));

            var centerAnimation = compositor.CreateVector3KeyFrameAnimation();
            centerAnimation.InsertExpressionKeyFrame(0f, "Vector3(startx,starty,0f)", standard);
            centerAnimation.InsertExpressionKeyFrame(1f, "Vector3(startx,starty,0f)", standard);
            centerAnimation.SetScalarParameter("startx", startx);
            centerAnimation.SetScalarParameter("starty", starty);
            centerAnimation.Duration = duration;

            var offsetAnimation = compositor.CreateVector2KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(0f, "-oldOffset", standard);
            offsetAnimation.InsertExpressionKeyFrame(1f, "This.StartingValue", standard);
            offsetAnimation.SetVector2Parameter("oldOffset", oldOffset);
            offsetAnimation.Duration = duration;

            var scaleAnimation = compositor.CreateVector2KeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(0f, "oldScale", standard);
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.StartingValue", standard);
            scaleAnimation.SetVector2Parameter("oldScale", oldScale);
            scaleAnimation.Duration = duration;

            ElementCompositionPreview.SetIsTranslationEnabled(newIndicator, true);

            new_target.StartAnimation("CenterPoint", centerAnimation);
            new_target.StartAnimation("Translation.XY", offsetAnimation);
            new_target.StartAnimation("Scale.XY", scaleAnimation);
        }

        private static void TryStartAnimationWithScale(FrameworkElement newIndicator, FrameworkElement oldIndicator)
        {
            var compositor = Window.Current.Compositor;

            var oldSize = new Vector2((float)oldIndicator.ActualWidth, (float)oldIndicator.ActualHeight);
            var newSize = new Vector2((float)newIndicator.ActualWidth, (float)newIndicator.ActualHeight);

            var oldScale = oldSize / newSize;

            var oldOffset = newIndicator.TransformToVisual(oldIndicator).TransformPoint(new Windows.Foundation.Point(0, 0)).ToVector2();

            float startx = 0, endx = 0, starty = 0, endy = 0;

            if (oldOffset.X > 0)
            {
                endx = newSize.X;
            }
            else
            {
                startx = newSize.X;
                oldOffset.X = oldOffset.X + newSize.X - oldSize.X;
            }

            if (oldOffset.Y > 0)
            {
                endy = newSize.Y;
            }
            else
            {
                starty = newSize.Y;
                oldOffset.Y = oldOffset.Y + newSize.Y - oldSize.Y;
            }

            var old_target = ElementCompositionPreview.GetElementVisual(oldIndicator);
            var new_target = ElementCompositionPreview.GetElementVisual(newIndicator);

            var duration = TimeSpan.FromSeconds(0.6d);

            var standard = compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));

            var singleStep = compositor.CreateStepEasingFunction();
            singleStep.IsFinalStepSingleFrame = true;

            var centerAnimation = compositor.CreateVector3KeyFrameAnimation();
            centerAnimation.InsertExpressionKeyFrame(0f, "Vector3(startx,starty,0f)", singleStep);
            centerAnimation.InsertExpressionKeyFrame(0.333f, "Vector3(endx,endy,0f)", singleStep);
            centerAnimation.SetScalarParameter("startx", startx);
            centerAnimation.SetScalarParameter("starty", starty);
            centerAnimation.SetScalarParameter("endx", endx);
            centerAnimation.SetScalarParameter("endy", endy);
            centerAnimation.Duration = duration;

            var offsetAnimation = compositor.CreateVector2KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(0f, "-oldOffset", singleStep);
            offsetAnimation.InsertExpressionKeyFrame(0.333f, "This.StartingValue", singleStep);
            offsetAnimation.SetVector2Parameter("oldOffset", oldOffset);
            offsetAnimation.Duration = duration;

            var scaleAnimation = compositor.CreateVector2KeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(0f, "oldScale", standard);
            scaleAnimation.InsertExpressionKeyFrame(0.333f, "(target.Size + abs(oldOffset)) / target.Size",
                compositor.CreateCubicBezierEasingFunction(c_frame1point1, c_frame1point2));
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.StartingValue",
                compositor.CreateCubicBezierEasingFunction(c_frame2point1, c_frame2point2));
            scaleAnimation.SetVector2Parameter("oldScale", oldScale);
            scaleAnimation.SetVector2Parameter("oldOffset", oldOffset);
            scaleAnimation.SetReferenceParameter("target", new_target);
            scaleAnimation.SetReferenceParameter("old", old_target);
            scaleAnimation.Duration = duration;

            ElementCompositionPreview.SetIsTranslationEnabled(newIndicator, true);

            new_target.StartAnimation("CenterPoint", centerAnimation);
            new_target.StartAnimation("Translation.XY", offsetAnimation);
            new_target.StartAnimation("Scale.XY", scaleAnimation);
        }

        #endregion Animation Methods

        #region Utilities

        private static FrameworkElement GetPivotHeaderIndicator(PivotHeaderPanel panel, string Name, int Index)
        {
            if (panel == null) return null;
            var container = panel.Children[Index] as FrameworkElement;
            return container?.VisualTreeFindName(Name);
        }

        private static bool IsLoaded(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            return element.ActualHeight > 0 || element.ActualWidth > 0;
        }

        #endregion Utilities
    }
}
