using WelldoneAuth.Lib.Models;
using WelldoneAuth.Lib.Models.Auth;

namespace WelldoneAuth.Lib.Interfaces
{
    public interface IAuthService
    {
        Task<CommonAPIModel<LoginRes>> PasswordLogin(PasswordLoginReq req);
        Task<CommonAPIModel<QrcodeLoginPrepareRes>> QrcodeLoginPrepare();
        Task<CommonAPIModel<LoginRes>> QrcodeLogin(QrcodeLoginReq req, Guid guid);
    }
}
