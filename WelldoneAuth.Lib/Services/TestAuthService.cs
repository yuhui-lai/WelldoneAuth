using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WelldoneAuth.Lib.Interfaces;
using WelldoneAuth.Lib.Models;
using WelldoneAuth.Lib.Models.Auth;

namespace WelldoneAuth.Lib.Services
{
    public class TestAuthService : IAuthService
    {
        private readonly ILogger<TestAuthService> logger;
        private readonly QrcodeSseNotifyService sseNotifyService;
        private readonly IConfiguration config;
        // 定義 IMemoryCache 變數
        private readonly IMemoryCache cache;
        private readonly string tempTokenKey = "temp-token";

        public TestAuthService(ILogger<TestAuthService> logger, QrcodeSseNotifyService sseNotifyService, IConfiguration config,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.sseNotifyService = sseNotifyService;
            this.config = config;
            this.cache = cache;
        }

        public async Task<CommonAPIModel<LoginRes>> PasswordLogin(PasswordLoginReq req)
        {
            throw new ApplicationException("顆顆");
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
                    QrcodeGuid = $"{config["QrcodeLogin:QrcodeContentTitle"]}:{guid}",
                    //QrcodeImg = QrcodeUtil.Create($"{config["QrcodeLogin:QrcodeContentTitle"]}:{guid}")
                    //LoginUrl = $"{apiPathBase}/api/TestAuth/QrcodeLogin/{guid}",
                    //SseUrl = $"{apiPathBase}/qrcode-login-sse/{guid}"
                }
            };
        }

        public async Task<CommonAPIModel<QrcodeLoginNotifyRes>> QrcodeLoginNotify(QrcodeLoginNotifyReq req, Guid guid)
        {
            // 驗證帳號跟device token
            if (req.DeviceToken == "123")
            {
                Guid tempToken = Guid.NewGuid();
                await sseNotifyService.Notify(guid, tempToken.ToString());
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                cache.Set($"{tempTokenKey}:{guid}", tempToken.ToString(), cacheEntryOptions);
                return new CommonAPIModel<QrcodeLoginNotifyRes>
                {
                    Data = new QrcodeLoginNotifyRes
                    {
                        TempToken = tempToken.ToString(),
                    }
                };
            }
            var errRes = new CommonAPIModel<QrcodeLoginNotifyRes>
            {
                Success = false,
                Msg = "登入失敗",
                Data = new QrcodeLoginNotifyRes()
            };
            await sseNotifyService.Notify(guid, JsonSerializer.Serialize(errRes));
            return errRes;
        }

        public async Task<CommonAPIModel<LoginRes>> QrcodeLogin(QrcodeLoginReq req)
        {
            var cacheKey = $"{tempTokenKey}:{req.Guid}";
            var msg = "";

            if (!cache.TryGetValue<string>(cacheKey, out var serverTempToken))
            {
                msg = "TempToken不存在";
            }
            else if (serverTempToken == req.TempToken)
            {
                cache.Remove(cacheKey);
                return new CommonAPIModel<LoginRes>
                {
                    Msg = "登入成功",
                    Data = new LoginRes
                    {
                        DisplayName = "Kobe"
                    }
                };
            }
            else
            {
                msg = "TempToken不正確";
            }
            cache.Remove(cacheKey);
            return new CommonAPIModel<LoginRes>
            {
                Success = false,
                Msg = msg,
                Data = new LoginRes()
            };
        }
    }
}
