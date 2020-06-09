using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BiliBili_UWP.Models.UI
{
    public class IncreaseCollection<T>:ObservableCollection<T>, ISupportIncrementalLoading
    {
        // 是否正在异步加载中
        private bool _isBusy = false;

        // 提供数据的 Func
        // 第一个参数：增量加载的起始索引；第二个参数：需要获取的数据量；第三个参数：获取到的数据集合
        private Func<IncreaseCollection<T>,Task<List<T>>> _funcGetData;
        // 最大可显示的数据量
        private uint _totalCount = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalCount">最大可显示的数据量</param>
        /// <param name="getDataFunc">提供数据的 Func</param>
        public IncreaseCollection(Func<IncreaseCollection<T>, Task<List<T>>> getDataFunc)
        {
            _funcGetData = getDataFunc;
        }

        /// <summary>
        /// 是否还有更多的数据
        /// </summary>
        public bool HasMoreItems { get; set; }

        /// <summary>
        /// 异步加载数据（增量加载）
        /// </summary>
        /// <param name="count">需要加载的数据量</param>
        /// <returns></returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_isBusy)
            {
                throw new InvalidOperationException("正在处理上一个请求");
            }
            _isBusy = true;

            var dispatcher = Window.Current.Dispatcher;

            return AsyncInfo.Run
            (
                (token) => Task.Run<LoadMoreItemsResult>
                (
                    async () =>
                    {
                        try
                        {
                            // 增量加载的起始索引
                            var startIndex = this.Count;
                            await dispatcher.RunAsync
                            (
                                 CoreDispatcherPriority.Normal,
                                 async () =>
                                 {
                                     // 通过 Func 获取增量数据
                                     var items = await _funcGetData(this).ConfigureAwait(true);
                                     if (items != null)
                                     {
                                         foreach (var item in items)
                                         {
                                             this.Add(item);
                                         }
                                     }
                                     else
                                     {
                                         throw new Exception("无法获取更多数据");
                                     }
                                     
                                 }
                             );
                            await Task.Delay(1000);
                            Debug.WriteLine(this.Count);
                            // Count - 实际已加载的数据量
                            return new LoadMoreItemsResult { Count = (uint)this.Count };
                        }
                        finally
                        {
                            _isBusy = false;
                        }
                    },
                    token
                )
            );
        }
    }
}
