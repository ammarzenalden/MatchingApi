using Matching.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendEmail(EmailRequestModel emailRequestModel)
        {
            try
            {
                string bb = $@"{emailRequestModel.Body}";
                Email email1 = new();
                email1.SendEmail(emailRequestModel.Email,emailRequestModel.Subject, bb);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        public class EmailRequestModel
        {
            public string? Email { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
        }
    }
}
