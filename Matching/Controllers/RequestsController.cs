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
            
            if (answerDto.Answer == true)
            {
                ticket!.TicketStatus = "Accepted";
                var allrequets = await _context.UserTickets.Where(x => x.SenderId == request.SenderId && x.ReceiverId != GetUserId()).ToListAsync();
                try
                {
                    foreach(var req in allrequets)
                    {
                        _context.UserTickets.Remove(req);
                    }
                }
                catch { }
            }
            else if(answerDto.Answer == false)
            {
                ticket!.TicketStatus = "Rejected";
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
