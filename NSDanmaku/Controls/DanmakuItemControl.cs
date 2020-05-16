using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp.UI.Controls;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NSDanmaku.Controls
{
    public  class DanmakuItemControl
    {
        /// <summary>
        /// 创建阴影弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Grid CreateControlShadow(float sizeZoom, bool bold, string fontFamily, DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();
            DropShadowPanel dropShadowPanel = new DropShadowPanel()
            {
                BlurRadius = 6,
                ShadowOpacity = 1,
                OffsetX = 0,
                OffsetY = 0,
                Color = GetBorderColor(model.color)
            };


            Grid grid = new Grid();



            tx.Text = model.text;
            if (bold)
            {
                tx.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                tx.FontFamily = new FontFamily(fontFamily);
            }
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;
            tx.FontSize = size;


            dropShadowPanel.Content = tx;

            grid.Children.Add(dropShadowPanel);
            grid.Tag = model;
            return grid;
        }
        /// <summary>
        /// 创建重叠弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Grid CreateControlOverlap(float sizeZoom, bool bold, string fontFamily, DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();
            TextBlock tx2 = new TextBlock();
            Grid grid = new Grid();


            tx2.Text = model.text;
            tx.Text = model.text;
            if (bold)
            {
                tx.FontWeight = FontWeights.Bold;
                tx2.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                tx.FontFamily = new FontFamily(fontFamily);
                tx2.FontFamily = new FontFamily(fontFamily);
            }
            tx2.Foreground = new SolidColorBrush(GetBorderColor(model.color));
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx2.FontSize = size;
            tx.FontSize = size;

            tx2.Margin = new Thickness(1);
            //grid包含弹幕文本信息

            grid.Children.Add(tx2);
            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        /// <summary>
        /// 创建无边框弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Grid CreateControlNoBorder(float sizeZoom, bool bold, string fontFamily, DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();

            Grid grid = new Grid();

            tx.Text = model.text;
            if (bold)
            {
                tx.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                tx.FontFamily = new FontFamily(fontFamily);
            }
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx.FontSize = size;

            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        /// <summary>
        /// 创建图片弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Grid CreateImageControl(BitmapImage image)
        {
            //创建基础控件
            Image img = new Image();
            img.Source = image;
            Grid grid = new Grid();
            DanmakuModel model = new DanmakuModel() { text = "666666" };
            grid.Tag = model;
            grid.Children.Add(img);
            return grid;
        }
        /// <summary>
        /// 创建边框弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<Grid> CreateControlBorder(float sizeZoom,bool bold, string fontFamily, DanmakuModel model)
        {

            float size = (float)model.size * (float)sizeZoom;

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasTextFormat fmt = new CanvasTextFormat() { FontSize = size };
            var tb = new TextBlock { Text = model.text, FontSize = size, };


            if (bold)
            {
                fmt.FontWeight = FontWeights.Bold;
                tb.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                fmt.FontFamily = fontFamily;
                tb.FontFamily = new FontFamily(fontFamily);
            }
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));


            var myBitmap = new CanvasRenderTarget(device, (float)tb.ActualWidth, (float)tb.ActualHeight, 96);

            CanvasTextLayout canvasTextLayout = new CanvasTextLayout(device, model.text, fmt, (float)tb.ActualWidth, (float)tb.ActualHeight);

            CanvasGeometry combinedGeometry = CanvasGeometry.CreateText(canvasTextLayout);

            using (var ds = myBitmap.CreateDrawingSession())
            {

                ds.FillGeometry(combinedGeometry, model.color);
                ds.DrawGeometry(combinedGeometry, GetBorderColor(model.color), 0.4f);
            }
            Image image = new Image();
            BitmapImage im = new BitmapImage();
            using (InMemoryRandomAccessStream oStream = new InMemoryRandomAccessStream())
            {
                // Save the Win2D canvas renderer a stream.
                await myBitmap.SaveAsync(oStream, CanvasBitmapFileFormat.Png, 1.0f);
                // Stream our Win2D pixels into the Bitmap
                await im.SetSourceAsync(oStream);
            }
            image.Source = im;
            image.Stretch = Stretch.None;
            Grid grid = new Grid();

            grid.Tag = model;
            grid.Children.Add(image);

            return grid;

        }

        public static Color GetBorderColor(Color textColor)
        {
            if (textColor.R <= 80)
            {
                return Colors.White;
            }
            else
            {
                return Colors.Black;
            }
            //if (textColor.R < 100 && textColor.G < 100 && textColor.B < 100)
            //{
            //    return Colors.White;
            //}
            //else if ((textColor.R > 200 && textColor.G < 100 && textColor.B < 100))
            //{
            //    return Colors.White;
            //}
            //else if ((textColor.R < 100 && textColor.G > 200 && textColor.B < 100))
            //{
            //    return Colors.White;
            //}
            //else if ((textColor.R < 100 && textColor.G < 100 && textColor.B > 200))
            //{
            //    return Colors.White;
            //}
            //else
            //{
            //    return Colors.Black;
            //}
        }
    }
}
