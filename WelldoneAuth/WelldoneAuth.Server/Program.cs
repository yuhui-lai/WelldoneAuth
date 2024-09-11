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

// ���U SSE �A��
builder.Services.AddServerSentEvents();
// ��� URL �]�t�� Guid ���o ClientId
builder.Services.AddSingleton<IServerSentEventsClientIdProvider, SseClientIdFromPathProvider>();
// �ϥε{���w���Ѫ� AddServerSentEvents �X�R��k���U
builder.Services.AddServerSentEvents<SseNotifyService, MemoryCacheSseNotifyService>(options =>
{
    // �{���w���� KeepAlive �\��
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

// �w�q SSE ��M���A�Ȥθ��ѤΫ��O�A�C�����ݪ����˾ާ@�����H�� GUID �ѧO
app.MapServerSentEvents<MemoryCacheSseNotifyService>("/qrcode-login-sse/{regex(^[=0-9a-z].+)$)}");
app.MapServerSentEvents("/default-sse");

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
