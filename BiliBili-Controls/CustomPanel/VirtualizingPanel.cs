using BiliBili_Controls.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliBili_Controls.CustomPanel
{
    public class VirtualizingPanel : Panel
    {
        private double _columnWidth;
        /// <summary>
        /// Gets or sets the desired width for each column.
        /// </summary>
        /// <remarks>
        /// The width of columns can exceed the DesiredColumnWidth if the HorizontalAlignment is set to Stretch.
        /// </remarks>
        public double DesiredColumnWidth
        {
            get { return (double)GetValue(DesiredColumnWidthProperty); }
            set { SetValue(DesiredColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DesiredColumnWidth"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DesiredColumnWidth"/> dependency property.</returns>
        public static readonly DependencyProperty DesiredColumnWidthProperty = DependencyProperty.Register(
            nameof(DesiredColumnWidth),
            typeof(double),
            typeof(VirtualizingPanel),
            new PropertyMetadata(250d, OnDesiredColumnWidthChanged));

        private static void OnDesiredColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (VirtualizingPanel)d;
            panel.InvalidateMeasure();
        }
        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的栈布局的间距.
        ///</summary>
        public Double ColumnSpacing
        {
            get { return (Double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public static readonly DependencyProperty ColumnSpacingProperty =
                DependencyProperty.Register(nameof(ColumnSpacing), typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10.0, OnColumnSpacingChanged));

        private static void OnColumnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ColumnSpacing), "VirtualizingPanel 的 ColumnSpacing 最小值为0.");
            }
            (d as VirtualizingPanel).RequestArrange();
        }

        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的子元素的间距.
        ///</summary>
        public Double ItemsSpacing
        {
            get { return (Double)GetValue(ItemsSpacingProperty); }
            set { SetValue(ItemsSpacingProperty, value); }
        }

        public static readonly DependencyProperty ItemsSpacingProperty =
                DependencyProperty.Register(nameof(ItemsSpacing), typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10.0, OnItemsSpacingChanged));

        private static void OnItemsSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ItemsSpacing), "VirtualizingPanel 的 ItemsSpacing 最小值为0.");
            }
            (d as VirtualizingPanel).RequestArrange();
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the Padding dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Padding"/> dependency property.</returns>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(VirtualizingPanel),
            new PropertyMetadata(default(Thickness), OnPaddingChanged));

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (VirtualizingPanel)d;
            panel.InvalidateMeasure();
        }

        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的布局方向.
        ///</summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
                DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingPanel), new PropertyMetadata(Windows.UI.Xaml.Controls.Orientation.Vertical, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VirtualizingPanel).RequestArrange();
        }

        /// <summary>
        /// 请求重新布局
        /// </summary>
        private void RequestArrange()
        {
            InvalidateMeasure(); //无效化测量
            InvalidateArrange(); //无效化布局
        }

        public static Rect GetArrange(DependencyObject obj)
        {
            return (Rect)obj.GetValue(ArrangeProperty);
        }

        public static void SetArrange(DependencyObject obj, Rect value)
        {
            obj.SetValue(ArrangeProperty, value);
        }

        public static readonly DependencyProperty ArrangeProperty =
            DependencyProperty.RegisterAttached("Arrange", typeof(Rect), typeof(ContentControl), new PropertyMetadata(Rect.Empty));

        private List<StackLayoutContext> layoutContext = new List<StackLayoutContext>();

        private int layoutRequest = -1;

        /// <summary>
        /// 布局初始化,初始化所有子项的布局
        /// </summary>
        private void LayoutInitialize(Size availableSize)
        {
            //清除布局上下文数据
            layoutContext.Clear();

            double availableWidth = availableSize.Width - Padding.Left - Padding.Right;
            double availableHeight = availableSize.Height - Padding.Top - Padding.Bottom;
            //计算子项固定边大小
            _columnWidth = Math.Min(DesiredColumnWidth, availableWidth);
            int numColumns = Math.Max(1, (int)Math.Floor(availableWidth / _columnWidth));
            double totalWidth = _columnWidth + ((numColumns - 1) * (_columnWidth + ColumnSpacing));
            if (totalWidth > availableWidth)
            {
                numColumns--;
            }
            else if (double.IsInfinity(availableWidth))
            {
                availableWidth = totalWidth;
            }
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                availableWidth = availableWidth - ((numColumns - 1) * ColumnSpacing);
                _columnWidth = availableWidth / numColumns;
            }
            double offset = ((Orientation == Orientation.Vertical ? availableWidth : availableHeight) - (numColumns * ColumnSpacing)) / numColumns;
            //为每个栈生成布局上下文数据
            for (int i = 0; i < numColumns; i++)
            {
                layoutContext.Add(new StackLayoutContext()
                {
                    //每个栈的子项的固定偏移,纵向排版就为 X 坐标,横向就为 Y 坐标
                    Offset = (offset + ColumnSpacing) * i,
                    Width = offset
                });
            }

            //遍历子项为每个子项进行测量
            foreach (var item in this.Children)
            {
                //寻找长度最短的栈
                StackLayoutContext minStack = layoutContext.First(c => c.Lenght == layoutContext.Min(s => s.Lenght));

                //根据不同的布局方向进行布局
                if (Orientation == Orientation.Vertical)
                {
                    //栈的固定长度,纵向排版为宽,横向为高
                    availableSize.Width = offset;
                    //子项的坐标,栈的目前长度与栈的固定偏移
                    Point coordinate = new Point(minStack.Offset, minStack.Lenght);
                    //测量子项所需尺寸
                    item.Measure(availableSize);
                    //将测量结果保存在附加属性 Arrange 中,以便布局时使用
                    SetArrange(item, new Rect(coordinate, item.DesiredSize));
                    //将最短栈加上当前子项的长度与子项间距
                    minStack.Lenght += item.DesiredSize.Height + ItemsSpacing;
                }
                else
                {
                    availableSize.Height = offset;
                    Point coordinate = new Point(minStack.Lenght, minStack.Offset);
                    item.Measure(availableSize);
                    SetArrange(item, new Rect(coordinate, item.DesiredSize));
                    minStack.Lenght += item.DesiredSize.Width + ItemsSpacing;
                }

                //判断子项是否实现 IVirtualizingItem 接口
                if (item is IVirtualizingItem)
                {
                    //判断子项是否应该被虚拟化
                    if (ShouldBeVirtualized(item))
                    {
                        (item as IVirtualizingItem).Virtualize();
                    }
                    else
                    {
                        if (minStack.FirstRealizedItemIndex == -1)
                        {
                            minStack.FirstRealizedItemIndex = minStack.Elements.Count;
                        }
                        minStack.RealizedItemCount++;
                        (item as IVirtualizingItem).Realize();
                    }
                }
                //添加子项到最短栈的结尾
                minStack.Elements.Add(item);
            }

            layoutRequest = 0;
        }

        /// <summary>
        /// 添加或移除一个元素布局
        /// </summary>
        private void LayoutItem(UIElement item)
        {
            //开始布局
            StackLayoutContext minStack = layoutContext.First(c => c.Lenght == layoutContext.Min(s => s.Lenght));

            if (Orientation == Orientation.Vertical)
            {
                Size availableSize = new Size(minStack.Width, double.PositiveInfinity);
                Point coordinate = new Point(minStack.Offset, minStack.Lenght);
                item.Measure(availableSize);
                SetArrange(item, new Rect(coordinate, item.DesiredSize));
                minStack.Lenght += item.DesiredSize.Height + ItemsSpacing;
            }
            else
            {
                Size availableSize = new Size(double.PositiveInfinity, minStack.Width);
                Point coordinate = new Point(minStack.Lenght, minStack.Offset);
                item.Measure(availableSize);
                SetArrange(item, new Rect(coordinate, item.DesiredSize));
                minStack.Lenght += item.DesiredSize.Width + ItemsSpacing;
            }

            if (item is IVirtualizingItem)
            {
                if (ShouldBeVirtualized(item))
                {
                    (item as IVirtualizingItem).Virtualize();
                }
                else
                {
                    if (minStack.FirstRealizedItemIndex == -1)
                    {
                        minStack.FirstRealizedItemIndex = minStack.Elements.Count;
                    }
                    minStack.RealizedItemCount++;
                    (item as IVirtualizingItem).Realize();
                }
            }
            minStack.Elements.Add(item);
            //完成一个子项布局,布局请求递减
            layoutRequest--;
        }

        private bool isVirtualizing = false;
        private ScrollViewerView scrollViewerFinalView;

        /// <summary>
        /// 开始虚拟化子项
        /// </summary>
        private void BeginItemsVirtualizing(ScrollViewerView view, ScrollViewerView final)
        {
            Rect viewport = new Rect(view.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, view.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);

            foreach (var context in layoutContext)
            {
                bool IsBeginRealize = false;

                foreach (var item in context.Elements)
                {
                    if (item is IVirtualizingItem)
                    {
                        if (ShouldBeVirtualized(item, viewport))
                        {
                            (item as IVirtualizingItem).Virtualize();
                            if (IsBeginRealize)
                            {
                                break;
                            }
                        }
                        else
                        {
                            IsBeginRealize = true;
                            (item as IVirtualizingItem).Realize();
                        }
                    }
                }
            }

        }
            
        /// <summary>
        /// 结束虚拟化子项
        /// </summary>
        private void EndItemsVirtualizing()
        {
            //虚拟化中
            if (isVirtualizing)
            {
                //遍历所有实例化的对象,对不需要的对象进行虚拟化
                foreach (var context in layoutContext)
                {
                    for (int i = context.FirstRealizedItemIndex; i < context.RealizedItemCount; i++)
                    {
                        if (context.Elements[i] is IVirtualizingItem)
                        {
                            if (ShouldBeVirtualized(context.Elements[i], scrollViewerFinalView))
                            {
                                (context.Elements[i] as IVirtualizingItem).Virtualize();
                            }
                        }
                    }
                }

                isVirtualizing = false;
            }
        }

        /// <summary>
        /// 判断一个元素是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        private bool ShouldBeVirtualized(UIElement item)
        {
            //获取元素的布局方块
            Rect arrange = GetArrange(item);
            //获取可视区域方块(上下左右均包含半个屏幕的缓冲区)
            Rect viewport = new Rect(ScrollOwner.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, ScrollOwner.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);
            //检查可视区域方块和元素布局方块是否有重叠
            viewport.Intersect(arrange);
            return viewport == Rect.Empty; //无重叠代表不在可视区域,需要进行虚拟化
        }

        /// <summary>
        /// 判断一个元素在指定的 ScrollViewerView 是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        /// <param name="view">指定的视图</param>
        /// <returns></returns>
        private bool ShouldBeVirtualized(UIElement item, ScrollViewerView view)
        {
            Rect arrange = GetArrange(item);
            Rect viewport = new Rect(view.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, view.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);
            viewport.Intersect(arrange);
            return viewport == Rect.Empty;
        }

        /// <summary>
        /// 判断一个元素在指定的 Rect 是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        /// <param name="viewRect">指定的视图</param>
        /// <returns></returns>
        private bool ShouldBeVirtualized(UIElement item, Rect viewRect)
        {
            Rect arrange = GetArrange(item);
            viewRect.Intersect(arrange);
            return viewRect == Rect.Empty;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //Hook子项生成器
            if (!isGeneratorHooked)
            {
                GetOwnerAndGenerator();
            }

            //有未完成的布局请求,一般出现在非队列最后插入子项,移除子项等操作,此时进行重新布局
            if (layoutRequest != 0)
            {
                LayoutInitialize(availableSize);
            }

            //返回控件大小
            if (Orientation == Orientation.Vertical)
            {
                return new Size(availableSize.Width, layoutContext.Max(s => s.Lenght));
            }
            else
            {
                return new Size(layoutContext.Max(s => s.Lenght), availableSize.Height);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var item in Children)
            {
                item.Arrange(GetArrange(item));
            }
            return finalSize;
        }

        private void ScrollOwnerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            BeginItemsVirtualizing(e.NextView, e.FinalView);
        }

        private void ItemContainerGeneratorItemsChanged(object sender, Windows.UI.Xaml.Controls.Primitives.ItemsChangedEventArgs e)
        {
            int index = -1;

            //判断 Items 的变更动作
            switch (e.Action)
            {
                //添加了项目
                case 1:
                    //将变更的项目加入布局请求
                    layoutRequest += e.ItemCount;

                    //判断是否是在最后加入了 Item
                    if ((index = ItemContainerGenerator.IndexFromGeneratorPosition(e.Position)) == ItemsOwner.Items.Count - 1)
                    {
                        //为加入的 Item 请求布局
                        LayoutItem(Children[index]);
                    }
                    break;

                //移除了项目
                case 2:
                    layoutRequest += e.ItemCount;
                    break;

                //未知操作
                default:
                    layoutRequest += e.ItemCount;
                    break;
            }
        }

        private ItemsControl itemsOwner; //子项的父控件
        private ScrollViewer scrollOwner; //面板所处的 ScrollView
        private ItemContainerGenerator itemContainerGenerator; //子项容器生成器
        private bool isGeneratorHooked;

        private void GetOwnerAndGenerator()
        {
            itemsOwner = ItemsControl.GetItemsOwner(this);
            if (itemsOwner == null)
            {
                throw new InvalidOperationException("无法从 VirtualizingPanel 找到父 ItemsControl.");
            }

            if (!(itemsOwner is ListViewBase))
            {
                throw new InvalidOperationException("VirtualizingPanel 应该在 ListView.ItemsPanel 中被调用");
            }

            scrollOwner = VisualTreeExtension.GetParentObject<ScrollViewer>(itemsOwner,"PageScrollViewer");
            if (scrollOwner == null)
            {
                throw new InvalidOperationException("无法从父 ItemsControl 找到 ScrollView");
            }

            itemContainerGenerator = itemsOwner.ItemContainerGenerator;
            if (itemContainerGenerator == null)
            {
                throw new InvalidOperationException("无法获取 VirtualizingPanel 所属 ItemsControl 的 ItemContainerGenerator");
            }

            if (!isGeneratorHooked)
            {
                isGeneratorHooked = true;
                scrollOwner.ViewChanging -= ScrollOwnerViewChanging;
                scrollOwner.ViewChanging += ScrollOwnerViewChanging;
                itemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;
                itemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
            }
        }

        public ItemsControl ItemsOwner
        {
            get
            {
                if (itemsOwner == null)
                {
                    GetOwnerAndGenerator();
                }
                return itemsOwner;
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                if (scrollOwner == null)
                {
                    GetOwnerAndGenerator();
                }
                return scrollOwner;
            }
        }

        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (itemContainerGenerator == null)
                {
                    GetOwnerAndGenerator();
                }
                return itemContainerGenerator;
            }
        }
    }
}
