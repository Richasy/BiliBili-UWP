using BiliBili_Controls.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliBili_Controls.Items
{
    public class VirtualizingItem : ListViewItem, IVirtualizingItem
    {


        public DataTemplate OriginTemplate
        {
            get { return (DataTemplate)GetValue(OriginTemplateProperty); }
            set { SetValue(OriginTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OriginTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginTemplateProperty =
            DependencyProperty.Register("OriginTemplate", typeof(DataTemplate), typeof(ListViewItem), new PropertyMetadata(null,new PropertyChangedCallback(OriginTemplate_Changed)));

        private static void OriginTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VirtualizingItem;
            instance.ContentTemplate = e.NewValue as DataTemplate;
        }


        /// <summary>
        /// 布局占用大小
        /// </summary>
        public Size RealSize
        {
            get { return (Size)GetValue(RealSizeProperty); }
            set { SetValue(RealSizeProperty, value); }
        }

        public static readonly DependencyProperty RealSizeProperty =
            DependencyProperty.Register(nameof(RealSize), typeof(Size), typeof(VirtualizingItem), new PropertyMetadata(Size.Empty));

        /// <summary>
        /// 是否处于虚拟化状态
        /// </summary>
        public bool IsVirtualized { get; private set; }

        /// <summary>
        /// 是否可进行虚拟化
        /// </summary>
        public bool CanVirtualize { get { return !IsVirtualized; } }

        /// <summary>
        /// 是否可进行实例化
        /// </summary>
        public bool CanRealize { get { return IsVirtualized; } }

        /// <summary>
        /// 判断是否是内部进行呈现内容变更
        /// </summary>
        private bool IsInternalChangeContent = false;

        public VirtualizingItem()
        {
            //this.DefaultStyleKey = typeof(VirtualizingItem);
        }

        private DataTemplate GetEmptyTemplate()
        {
            return new DataTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsVirtualized)
            {
                //虚拟化时固定大小
                return RealSize;
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        /// <summary>
        /// 虚拟化子项
        /// </summary>
        public void Virtualize()
        {
            OnVirtualize();
        }

        /// <summary>
        /// 实例化子项
        /// </summary>
        public void Realize()
        {
            OnRealize();
        }

        /// <summary>
        /// 可重写,实现虚拟化子项的过程
        /// </summary>
        protected virtual void OnVirtualize()
        {
            if (!IsVirtualized)
            {
                IsInternalChangeContent = true;
                this.IsVirtualized = true;
                RealSize = this.DesiredSize;
                //从可视化树移除内容
                this.ContentTemplate = GetEmptyTemplate();
            }
        }

        /// <summary>
        /// 可重新,实现实例化子项的过程
        /// </summary>
        protected virtual void OnRealize()
        {
            if (IsVirtualized)
            {
                IsInternalChangeContent = true;
                this.IsVirtualized = false;
                //从可视化树呈现内容
                this.ContentTemplate = OriginTemplate;
            }
        }
    }
}
