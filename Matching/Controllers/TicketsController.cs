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
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        [HttpPost("CreateTicket")]
        public async Task<ActionResult> CreateTicket(TicketDto ticketDto)
        {
            var oldTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId()); 
            if(oldTicket != null) {
                return Conflict(new
                {
                    success = false,
                    message = "you already have Ticket"
                });
            }
            DateTime.TryParseExact(ticketDto.BookingDate, "yyyy-MM-dd hhhtt", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);
            var RoomDate = await _context.Tickets
                .Where(x => x.RoomId == ticketDto.RoomId && x.BookingDate == dateTime).ToListAsync();
            if(RoomDate!.Any())
            {
                return Conflict(new
                {
                    success = false,
                    message = "the Room is booked on this date chose another date",
                    ticket = RoomDate
                });
            }
            
            Ticket ticket = new()
            {
                Type = ticketDto.Type,
                BookingDate = dateTime,
                CreatorId = GetUserId(),
                RoomId = ticketDto.RoomId
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = ticket
            });
        }
        [HttpPut("UpdateTicket")]
        public async Task<ActionResult> UpdateTicket(TicketDto ticketDto)
        {
            var oldTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId());
            if(oldTicket == null) 
            {
                return NotFound(new
                {
                    success = false,
                    message = "you dont have ticket"
                });
            }
            DateTime.TryParseExact(ticketDto.BookingDate, "yyyy-MM-dd hhhtt", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);

            var RoomDate = await _context.Tickets
               .Where(x => x.RoomId == ticketDto.RoomId && x.BookingDate == dateTime).ToListAsync();
            if (RoomDate != null)
            {
                return Conflict(new
                {
                    success = false,
                    message = "the Room is booked on this date chose another date"
                });
            }
            oldTicket.BookingDate = dateTime;
            oldTicket.RoomId = ticketDto.RoomId;
            oldTicket.Type = ticketDto.Type;
            _context.Tickets.Update(oldTicket);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = oldTicket
            });
        }
        [HttpDelete("DeleteTicket")]
        public async Task<ActionResult> DeleteTicket()
        {
            var oldTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId());
            if (oldTicket == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you dont have ticket"
                });
            }
            _context.Tickets.Remove(oldTicket);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Deleted"
            });
        }
        [HttpGet("GetUserTicket")]
        public async Task<ActionResult> GetUserTicket()
        {
            var userTickets =await _context.UserTickets.Where(x => x.SenderId == GetUserId()).ToListAsync();
            return Ok(new
            {
                success = true,
                data = userTickets
            });
        }
    }
}
