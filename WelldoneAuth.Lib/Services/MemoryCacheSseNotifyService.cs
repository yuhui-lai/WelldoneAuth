using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WelldoneAuth.Lib.Services
{
    public class MemoryCacheSseNotifyService : QrcodeSseNotifyService, IServerSentEventsService
    {
        // 定義 IMemoryCache 變數
        private readonly IMemoryCache cache;
        // 定義快取鍵值
        private readonly string sseCacheGuidKey = "sse-qrcode-guid";
        private readonly string sseCacheMsgKey = "sse-qrcode-msg";

        // 建構子，初始化快取和基底類別
        public MemoryCacheSseNotifyService(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options,
            IMemoryCache cache) :
            base(options.ToBaseServerSentEventsServiceOptions<ServerSentEventsService>())
        {
            this.cache = cache;
        }

        // 覆寫 Subscribe 方法，訂閱特定的 GUID
        public override void Subscribe(Guid guid, int timeoutSecs = 60)
        {
            // 建立快取鍵值
            var key = $"{sseCacheGuidKey}:{guid}";
            // 初始化信號量
            var semaphore = new SemaphoreSlim(0);
            // 設定超時時間
            var timeout = TimeSpan.FromSeconds(timeoutSecs);
            // 將信號量存入快取
            cache.Set(key, semaphore, timeout);
            // 啟動新任務
            var task = Task.Factory.StartNew(async () =>
            {
                // 等待信號量釋放
                if (!await semaphore.WaitAsync(TimeSpan.FromSeconds(timeoutSecs)))
                    // 超時處理
                    await SendEventAsync(guid, "error", "Timeout");
                // 嘗試從快取中取得回應
                else if (!cache.TryGetValue<string>($"{sseCacheMsgKey}:{guid}", out var res))
                    // 無回應處理
                    await SendEventAsync(guid, "error", "No response");
                else
                {
                    // 發送回應訊息
                    await SendEventAsync(guid, "message", res);
                    cache.Remove($"{sseCacheMsgKey}:{guid}");
                }
                    
            });
        }

        // 覆寫 Notify 方法，通知特定的 GUID
        public async override Task Notify(Guid guid, string message)
        {
            // 建立快取鍵值
            var key = $"{sseCacheGuidKey}:{guid}";
            // 嘗試從快取中取得信號量
            if (!cache.TryGetValue(key, out SemaphoreSlim semaphore))
                throw new ApplicationException("Token not found");
            // 將訊息存入快取
            cache.Set($"{sseCacheMsgKey}:{guid}", message);
            // 釋放信號量
            semaphore.Release();
            // 移除快取中的信號量
            cache.Remove(key);
        }
    }
}