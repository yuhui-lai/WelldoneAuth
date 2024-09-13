using WelldoneAuth.Lib.Models;
using WelldoneAuth.Lib.Models.Auth;

namespace WelldoneAuth.Lib.Interfaces
{
    public interface IAuthService
    {
        Task<CommonAPIModel<LoginRes>> PasswordLogin(PasswordLoginReq req);
        /// <summary>
        /// QRcode登入分3步
        /// 1. 生成guid，以guid建立server sent event api
        /// </summary>
        /// <returns></returns>
        Task<CommonAPIModel<QrcodeLoginPrepareRes>> QrcodeLoginPrepare();
        /// <summary>
        /// 2. 手機app由sse/guid發佈帳號與device token
        /// </summary>
        /// <param name="req"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<CommonAPIModel<QrcodeLoginNotifyRes>> QrcodeLoginNotify(QrcodeLoginNotifyReq req, Guid guid);
        /// <summary>
        /// 3. client從sse訂閱取得帳號與device token去做登入
        /// </summary>
        /// <param name="req"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<CommonAPIModel<LoginRes>> QrcodeLogin(QrcodeLoginReq req);
    }
}
