using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WelldoneAuth.Lib.Models;

namespace WelldoneAuth.Lib.Extensions
{
    public static class WelldoneExceptionHandler
    {
        public static void UseWelldoneExceptHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionFeature?.Error != null)
                    {
                        var logger = loggerFactory.CreateLogger("WelldoneExceptionHandler");
                        logger.LogError(exceptionFeature.Error, exceptionFeature.Error.Message);
                    }

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    var result = JsonSerializer.Serialize(new CommonAPIModel<string>
                    {
                        Success = false,
                        Msg = "系統忙碌中，請稍後重試",
                        Data = "系統忙碌中，請稍後重試"
                    });
                    await context.Response.WriteAsync(result);
                }
            });
        }
    }
}
