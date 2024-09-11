using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using WelldoneAuth.Lib.Interfaces;
using WelldoneAuth.Lib.Models;
using WelldoneAuth.Lib.Models.Auth;
using WelldoneAuth.Lib.Utilities;

namespace WelldoneAuth.Lib.Services
{
    public class TestAuthService : IAuthService
    {
        private readonly ILogger<TestAuthService> logger;
        private readonly SseNotifyService sseNotifyService;
        private readonly IConfiguration config;

        public TestAuthService(ILogger<TestAuthService> logger, SseNotifyService sseNotifyService, IConfiguration config) 
        { 
            this.logger = logger;
            this.sseNotifyService = sseNotifyService;
            this.config = config;
        }

        public async Task<CommonAPIModel<LoginRes>> PasswordLogin(PasswordLoginReq req)
        {
            if (req.Username == "marco" && req.Password == "123")
            {
                return new CommonAPIModel<LoginRes>
                {
                    Data = new LoginRes
                    {
                        DisplayName = "Marco"
                    }
                };
            }
            return new CommonAPIModel<LoginRes>
            {
                Success = false,
                Msg = "請檢查帳密",
                Data = new LoginRes()
            };
        }

        public async Task<CommonAPIModel<QrcodeLoginPrepareRes>> QrcodeLoginPrepare()
        {
            var guid = Guid.NewGuid();
            int timeout = config.GetValue<int>("QrcodeLogin:QrcodeSseTimeout");
            sseNotifyService.Subscribe(guid, timeout);
            return new CommonAPIModel<QrcodeLoginPrepareRes>
            {
                Data = new QrcodeLoginPrepareRes
                {
                    Guid = guid,
                    QrcodeImg = QrcodeUtil.Create($"{config["QrcodeLogin:QrcodeContentTitle"]}:{guid}")
                    //LoginUrl = $"{apiPathBase}/api/TestAuth/QrcodeLogin/{guid}",
                    //SseUrl = $"{apiPathBase}/qrcode-login-sse/{guid}"
                }
            };
        }

        public async Task<CommonAPIModel<LoginRes>> QrcodeLogin(QrcodeLoginReq req, Guid guid)
        {
            if (req.DeviceToken == "123")
            {
                LoginRes loginRes = new()
                {
                    DisplayName = req.Username//"Marco"
                };
                await sseNotifyService.Notify(guid, JsonSerializer.Serialize(loginRes));

                return new CommonAPIModel<LoginRes>
                {
                    Data = loginRes
                };
            }
            var errRes = new CommonAPIModel<LoginRes>
            {
                Success = false,
                Msg = "登入失敗",
                Data = new LoginRes()
            };
            await sseNotifyService.Notify(guid, JsonSerializer.Serialize(errRes));
            return errRes;
        }
    }
}
