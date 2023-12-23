using Matching.Configure;
using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Http;
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
        [HttpPost("SendRequest/{id}")]
        public async Task<ActionResult> SendRequset(int id)
        {
            var receiver = await _context.Users.FindAsync(id);
            if(receiver == null)
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
            userTicket!.TicketStatus = "wating";
            _context.UserTickets.Update(userTicket);
            await _context.SaveChangesAsync();
            Email email = new();
            try
            {
                //change the body to html when the user has received new friend request 
                //for example : body = @"<html><body>....</body></html>";
                string body = @"";
                //change the subject to suitable one 
                //for example : subject = "new request";
                string subject = "";
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
            return Ok(new
            {
                success = true,
                data = sendedRequests
            });
        }
        [HttpPut("AnswerTheRequest")]
        public async Task<ActionResult> AnswerTheRequest(AnswerDto answerDto)
        {
            var request = await _context.Requests.FindAsync(answerDto.RequestId);
            if(request == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "there is no request by this id"
                });
            }
            if(request.ReceiverId != GetUserId())
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
            if (answerDto.Answer == true)
            {
                ticket!.TicketStatus = "Accepted";
                var allrequets = await _context.UserTickets.Where(x => x.SenderId == request.SenderId && x.ReceiverId != GetUserId()).ToListAsync();
                if(allrequets.Count != 0)
                {
                    _context.UserTickets.RemoveRange(allrequets);
                    
                }
                try
                {
                    //change the body to html if the user Accept the request 
                    //for example : body = @"<html><body>....</body></html>";
                    string body = @"";
                    //change the subject to suitable one 
                    //for example : subject = "Accepted";
                    string subject = "";
                    email.SendEmail(sender!.Email!, subject, body);
                }
                catch
                {

                }
            }
            else if(answerDto.Answer == false)
            {
                ticket!.TicketStatus = "Rejected";
                try
                {
                    //change the body to html if the user reject the request 
                    //for example : body = @"<html><body>....</body></html>";
                    string body = @"";
                    //change the subject to suitable one 
                    //for example : subject = "Rejected";
                    string subject = "";
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
