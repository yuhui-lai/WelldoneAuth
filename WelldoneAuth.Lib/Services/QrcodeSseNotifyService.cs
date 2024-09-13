using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace WelldoneAuth.Lib.Services
{
    public abstract class QrcodeSseNotifyService : ServerSentEventsService, IServerSentEventsService
    {
        public QrcodeSseNotifyService(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options)
            : base(options.ToBaseServerSentEventsServiceOptions<ServerSentEventsService>())
        { }

        public abstract void Subscribe(Guid token, int timeoutSecs = 60);

        public async Task SendEventAsync(Guid token, string type, string message)
        {
            var client = this.GetClients().SingleOrDefault(o =>
                o.Id == token);
            if (client != null)
            {
                await client.SendEventAsync(new ServerSentEvent()
                {
                    Type = type,
                    Data = new List<string> { message }
                });
            }
        }

        public abstract Task Notify(Guid token, string message);
    }
}
