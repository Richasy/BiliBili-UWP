using BiliBili_Controls.Extensions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;

namespace BiliBili_Controls.Bubble
{
    public class Bubble : IDisposable
    {
        public Bubble(Compositor Compositor, CanvasDevice canvasDevice, CompositionGraphicsDevice graphicsDevice, Size TargetSize, Color Color, TimeSpan Duration, bool OnTop, Size? Size = null, bool? IsFill = null)
        {
            _compositor = Compositor;
            _canvasDevice = canvasDevice;
            _graphicsDevice = graphicsDevice;
            _visual = Compositor.CreateSpriteVisual();

            if (!IsFill.HasValue)
            {
                var tmp = rnd.Next(2);
                IsFill = tmp > 0;
            }

            if (Size.HasValue)
            {
                this.Size = Size.Value.ToVector2();
            }
            else
            {
                var maxRadius = (int)Math.Min(TargetSize.Width, TargetSize.Height);
                if (IsFill.Value)
                {
                    this.Size = new Vector2(rnd.Next(maxRadius / 7, maxRadius / 4));
                }
                else
                {
                    this.Size = new Vector2(rnd.Next(maxRadius / 6, maxRadius / 3));
                }
            }
            Draw(IsFill.Value, Color);

            Offset = new Vector3((float)TargetSize.Width / 2, (float)TargetSize.Height / 2, 0f);
            _visual.Size = this.Size;
            _visual.Offset = Offset;
            _visual.Scale = Vector3.Zero;
            _visual.BindCenterPoint();
            CreateAnimation(TargetSize, _visual.Offset, OnTop, Duration);
        }

        ~Bubble()
        {
            Dispose(false);
        }

        private readonly static Random rnd = new Random();

        private static CompositionEasingFunction easing;

        private Compositor _compositor;
        private CanvasDevice _canvasDevice;
        private CompositionGraphicsDevice _graphicsDevice;
        private SpriteVisual _visual;
        private CompositionAnimationGroup _animations;
        private Vector2 Size;
        private Vector3 Offset;
        private CompositionDrawingSurface _surface;
        private CompositionSurfaceBrush _brush;


        private void Draw(bool IsFill, Color color)
        {
            _surface = _graphicsDevice.CreateDrawingSurface(Size.ToSize(), Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
            using (var dc = CanvasComposition.CreateDrawingSession(_surface))
            {
                dc.Clear(Colors.Transparent);
                if (IsFill)
                {
                    dc.FillCircle(Size / 2, Size.X / 2, color);
                }
                else
                {
                    dc.DrawCircle(Size / 2, Size.X / 2 - 2, color, 2f);
                }
                dc.Flush();
            }
            _brush = _compositor.CreateSurfaceBrush(_surface);
            _visual.Brush = _brush;
        }

        private void CreateAnimation(Size TargetSize, Vector3 StartOffset, bool OnTop, TimeSpan Duration)
        {
            if (easing == null)
            {
                easing = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.17f, 0.67f), new Vector2(0.44f, 0.999f));
            }

            _animations = _compositor.CreateAnimationGroup();

            var scalean = _compositor.CreateVector3KeyFrameAnimation();
            scalean.InsertKeyFrame(0f, Vector3.Zero);
            scalean.InsertKeyFrame(0.2f, Vector3.One, easing);
            scalean.InsertKeyFrame(1f, new Vector3(0.06f, 0.06f, 1f), easing);
            scalean.Duration = Duration;
            scalean.Target = "Scale";
            scalean.StopBehavior = AnimationStopBehavior.SetToInitialValue;

            var offsetan = _compositor.CreateVector3KeyFrameAnimation();
            offsetan.InsertKeyFrame(0f, StartOffset, easing);
            if (OnTop)
            {
                //在上半部分
                offsetan.InsertKeyFrame(1f, new Vector3(rnd.Next(-(int)TargetSize.Width, (int)TargetSize.Width) + (int)TargetSize.Width / 2, rnd.Next(-(int)(TargetSize.Height * 1.5), 0) + (int)TargetSize.Height / 2, 0f), easing);
            }
            else
            {
                //在下半部分
                offsetan.InsertKeyFrame(1f, new Vector3(rnd.Next(-(int)TargetSize.Width, (int)TargetSize.Width) + (int)TargetSize.Width / 2, rnd.Next(0, (int)(TargetSize.Height * 1.5)) + (int)TargetSize.Height / 2, 0f), easing);
            }
            offsetan.Duration = Duration;
            offsetan.Target = "Offset";
            offsetan.StopBehavior = AnimationStopBehavior.SetToInitialValue;

            _animations.Add(scalean);
            _animations.Add(offsetan);
        }

        public void AddTo(ContainerVisual container)
        {
            container.Children.InsertAtBottom(_visual);
        }

        public void Start()
        {
            _visual.StopAnimationGroup(_animations);
            _visual.StartAnimationGroup(_animations);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool IsDisposing)
        {
            _visual?.Dispose();
            _visual = null;

            _brush?.Dispose();
            _brush = null;

            _surface?.Dispose();
            _surface = null;

            _animations?.Dispose();
            _animations = null;

            if (IsDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
