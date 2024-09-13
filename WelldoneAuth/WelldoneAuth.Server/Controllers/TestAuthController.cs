using Microsoft.AspNetCore.Mvc;
using WelldoneAuth.Lib.Interfaces;
using WelldoneAuth.Lib.Models.Auth;

namespace WelldoneAuth.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestAuthController : ControllerBase
    {
        private IAuthService authService;

        public TestAuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> PasswordLogin([FromBody] PasswordLoginReq req)
        {
            var res = await authService.PasswordLogin(req);
            if (res.Success)
            {
                return Ok(res);
            }
            return NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> QrcodeLoginPrepare()
        {
            //string apiPathBase = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var res = await authService.QrcodeLoginPrepare();
            if (res.Success)
            {
                return Ok(res);
            }
            return NotFound(res);
        }

        [HttpPost("{guid}")]
        public async Task<IActionResult> QrcodeLoginNotify([FromBody]QrcodeLoginNotifyReq req, [FromRoute]Guid guid)
        {
            var res = await authService.QrcodeLoginNotify(req, guid);
            if (res.Success)
            {
                return Ok(res);
            }
            return NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> QrcodeLogin([FromBody] QrcodeLoginReq req)
        {
            var res = await authService.QrcodeLogin(req);
            if (res.Success)
            {
                return Ok(res);
            }
            return NotFound(res);
        }
    }
}
