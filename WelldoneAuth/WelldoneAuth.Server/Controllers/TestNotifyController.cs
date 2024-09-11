using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;
using WelldoneAuth.Lib.Models;

namespace WelldoneAuth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestNotifyController : ControllerBase
    {
        private readonly IServerSentEventsService serverSentEventsService;

        public TestNotifyController(IServerSentEventsService serverSentEventsService)
        {
            this.serverSentEventsService = serverSentEventsService;
        }

        [HttpGet]
        public async Task<IActionResult> Notify([FromQuery] string msg)
        {
            await serverSentEventsService.SendEventAsync(msg);
            return Ok(new CommonAPIModel<string>
            {
                Data = msg,
            });
        }
    }
}
