using Matching.Configure;
using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        private string GetUserName()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

            return userId!;
        }
        [HttpPost("SendRequest/{id}")]
        public async Task<ActionResult> SendRequset(int id)
        {
            var receiver = await _context.Users.FindAsync(id);
            if (receiver == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "there is no user by this Id"
                });
            }
            Request request = new()
            {
                DateTime = DateTime.UtcNow,
                SenderId = GetUserId(),
                ReceiverId = id,
            };
            _context.Requests.Add(request);
            var userTicket = await _context.UserTickets
                .FirstOrDefaultAsync(x => x.ReceiverId == id && x.SenderId == GetUserId());

            userTicket!.TicketStatus = "waiting";
            _context.UserTickets.Update(userTicket);
            string senderName = GetUserName();
            await _context.SaveChangesAsync();
            Email email = new();
            try
            {
                //change the body to html when the user has received new friend request 
                //for example : body = @"<html><body>....</body></html>";
                string body = $@"<html><body><div style=""width: 100%;padding: 30px 0px;background-color: #FFE2C1;"">
                                <div>
                                <h1 style=""text-align: center;font-size:1.8rem"">New Invitation Request</h1>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">you have new invitation request from {senderName}</p><br>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Login and Check It Now </p><br>      
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">On  <a href='https://love-lockdown.vercel.app/'>Love Lockdown</a></p><br>
                                </div>
                                </div></body></html>";
                //change the subject to suitable one 
                //for example : subject = "new request";
                string subject = "New Invitation Request";
                email.SendEmail(receiver.Email!, subject, body);

            }
            catch
            {

            }
            return Ok(new
            {
                success = true,
                data = request
            });
        }
        [HttpGet("GetSendedRequests")]
        public async Task<ActionResult> GetSendedRequests()
        {
            var sendedRequests = await _context.Requests.Where(x => x.SenderId == GetUserId()).ToListAsync();
            return Ok(new
            {
                success = true,
                data = sendedRequests
            });
        }
        [HttpGet("GetReceivedRequests")]
        public async Task<ActionResult> GetReceivedRequests()
        {
            var sendedRequests = await _context.Requests.Where(x => x.ReceiverId == GetUserId()).ToListAsync();

            List<Tuple<Request, UserResultDto>> aa = new();
            if (sendedRequests.Count > 0)
            {
                foreach (var ss in sendedRequests)
                {
                    var user = await _context.Users.FindAsync(ss.SenderId);
                    UserResultDto userResultDto = new()
                    {
                        Email = user!.Email,
                        Id = user.Id,
                        ImageUrl = user.ImageUrl,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber
                    };
                    aa.Add(new Tuple<Request, UserResultDto>(ss, userResultDto));
                }

            }

            return Ok(new
            {
                success = true,
                data = aa
            });
        }
        [HttpPut("AnswerTheRequest")]
        public async Task<ActionResult> AnswerTheRequest(AnswerDto answerDto)
        {
            var request = await _context.Requests.FindAsync(answerDto.RequestId);
            if (request == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "there is no request by this id"
                });
            }
            if (request.ReceiverId != GetUserId())
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "you are not the receiver "
                });
            }
            var ticket = await _context.UserTickets
                .FirstOrDefaultAsync(x => x.SenderId == request.SenderId && x.ReceiverId == GetUserId());
            Email email = new();
            var sender = await _context.Users.FindAsync(request.SenderId);
            string userName = GetUserName();
            if (answerDto.Answer == true)
            {
                ticket!.TicketStatus = "Accepted";
                var allrequets = await _context.UserTickets.Where(x => x.SenderId == request.SenderId && x.ReceiverId != GetUserId()).ToListAsync();
                if (allrequets.Count != 0)
                {
                    _context.UserTickets.RemoveRange(allrequets);

                }
                try
                {
                    //change the body to html if the user Accept the request 
                    //for example : body = @"<html><body>....</body></html>";
                    string body = $@"<html><body><div style=""width: 100%;padding: 30px 0px;background-color: #FFE2C1;"">
                                <div>
                                <h1 style=""text-align: center;font-size:1.8rem;text-transform: capitalize"">invitation request Accepted</h1>
                                <p style=""text-align: center;font-size:1.1rem;text-transform: capitalize;padding:0px 10px""> {userName} Accept you'r invitation request  </p><br>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Login and Check It Now </p><br>      
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">On  <a href='https://love-lockdown.vercel.app/'>Love Lockdown</a></p><br>
                                </div>
                                </div></body></html>";
                    //change the subject to suitable one 
                    //for example : subject = "Accepted";
                    string subject = "Invitation Request Accepted";
                    email.SendEmail(sender!.Email!, subject, body);
                }
                catch
                {

                }
            }
            else if (answerDto.Answer == false)
            {
                ticket!.TicketStatus = "Rejected";
                try
                {
                    //change the body to html if the user reject the request 
                    //for example : body = @"<html><body>....</body></html>";
                    string body = $@"<html><body><div style=""width: 100%;padding: 30px 0px;background-color: #FFE2C1;"">
                                <div>
                                <h1 style=""text-align: center;font-size:1.8rem;text-transform: capitalize"">invitation request Rejected</h1>
                                <p style=""text-align: center;font-size:1.1rem;text-transform: capitalize;padding:0px 10px""> {userName} Reject you'r invitation request  </p><br>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Login and Look for New Partner </p><br>      
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">On  <a href='https://love-lockdown.vercel.app/'>Love Lockdown</a></p><br>
                                </div>
                                </div></body></html>";
                    //change the subject to suitable one 
                    //for example : subject = "Rejected";
                    string subject = "Invitation Request Rejected";
                    email.SendEmail(sender!.Email!, subject, body);
                }
                catch
                {

                }
            }
            _context.UserTickets.Update(ticket!);
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = ticket
            });

        }
    }
}
