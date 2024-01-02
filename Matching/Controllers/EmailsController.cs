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
        public ActionResult SendEmail(string email,string subj,string body)
        {
            try
            {
                string bb = $@"{body}";
                Email email1 = new();
                email1.SendEmail(email,subj, bb);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
