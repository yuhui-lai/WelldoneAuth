using Lib.AspNetCore.ServerSentEvents;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WelldoneAuth.Lib.Interfaces;
using WelldoneAuth.Lib.Providers;
using WelldoneAuth.Lib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddMemoryCache();

// 註冊 SSE 服務
builder.Services.AddServerSentEvents();
// 改由 URL 包含的 Guid 取得 ClientId
builder.Services.AddSingleton<IServerSentEventsClientIdProvider, SseClientIdFromPathProvider>();
// 使用程式庫提供的 AddServerSentEvents 擴充方法註冊
builder.Services.AddServerSentEvents<SseNotifyService, MemoryCacheSseNotifyService>(options =>
{
    // 程式庫提供 KeepAlive 功能
    options.KeepaliveMode = ServerSentEventsKeepaliveMode.Always;
    options.KeepaliveInterval = 15;
});

builder.Services.AddScoped<IAuthService, TestAuthService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

// 定義 SSE 對映的服務及路由及型別，每次等待的掃瞄操作產生隨機 GUID 識別
app.MapServerSentEvents<MemoryCacheSseNotifyService>("/qrcode-login-sse/{regex(^[=0-9a-z].+)$)}");
app.MapServerSentEvents("/default-sse");

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
