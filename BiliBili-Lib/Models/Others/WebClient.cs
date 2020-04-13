using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace BiliBili_Lib.Models.Others
{
    [AllowForWeb]
    public sealed class BiliWebClient
    {
        public event EventHandler<string> ValidateLoginEvent;
        public void ValidateLogin(string data)
        {
            ValidateLoginEvent?.Invoke(this, data);
        }
        public event EventHandler CloseWebViewEvent;
        public void CloseBrowser()
        {
            CloseWebViewEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
