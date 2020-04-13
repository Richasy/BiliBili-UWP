using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace BiliBili_Lib.Models.Others
{
    [AllowForWeb]
    public sealed class BiliSecure
    {
        public event EventHandler CaptchaEvent;
        public void Captcha()
        {
            CaptchaEvent?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseCaptchaEvent;
        public void CloseCaptcha()
        {
            CloseCaptchaEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
