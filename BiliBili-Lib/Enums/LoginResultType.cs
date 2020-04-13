using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Enums
{
    public enum LoginResultType
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        Success,
        /// <summary>
        /// 登录失败
        /// </summary>
        Fail,
        /// <summary>
        /// 登录错误
        /// </summary>
        Error,
        /// <summary>
        /// 登录需要验证码
        /// </summary>
        NeedCaptcha,
        /// <summary>
        /// 需要安全认证
        /// </summary>
        NeedValidate,
        Busy
    }
}
