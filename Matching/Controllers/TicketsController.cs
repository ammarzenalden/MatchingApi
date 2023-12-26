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
        [HttpGet("GetTicket")]
        public async Task<ActionResult> GetTicket()
        {
            var pp = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId());
            return Ok(new
            {
                success = true,
                data = pp
            });
        }
        [HttpPost("CreateTicket")]
        public async Task<ActionResult> CreateTicket(TicketDto ticketDto)
        {
            var oldTicket = await _context.Tickets.Where(x => x.CreatorId == GetUserId()).ToListAsync();
            
            if(oldTicket.Count>0) {
                foreach (var item in oldTicket)
                {
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && x.TicketStatus == "done");
                    if (oldUserTicket == null)
                    {
                        return Conflict(new
                        {
                            success = false,
                            message = "you already have Ticket"
                        }); 
                    }
                    
                }
            }
            var RoomDate = await _context.Tickets
                .Where(x => x.RoomId == ticketDto.RoomId && x.BookingDate == ticketDto.BookingDate).ToListAsync();
            if (RoomDate.Count > 0)
            {
                foreach (var item in RoomDate)
                {
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && x.TicketStatus == "done");
                    if (oldUserTicket == null)
                    {
                        return Conflict(new
                        {
                            success = false,
                            message = "the Room is booked on this date chose another date",
                            ticket = RoomDate
                        });
                    }

                }
            }
            //if (RoomDate.Count != 0)
            //{
            //    return Conflict(new
            //    {
            //        success = false,
            //        message = "the Room is booked on this date chose another date",
            //        ticket = RoomDate
            //    });
            //}
            
            Ticket ticket = new()
            {
                Type = ticketDto.Type,
                BookingDate = ticketDto.BookingDate,
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
            //DateTime.TryParseExact(ticketDto.BookingDate, "yyyy-MM-dd hhhtt", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);

            var RoomDate = await _context.Tickets
               .Where(x => x.RoomId == ticketDto.RoomId && x.BookingDate == ticketDto.BookingDate).ToListAsync();
            if (RoomDate.Count != 0)
            {
                return Conflict(new
                {
                    success = false,
                    message = "the Room is booked on this date chose another date",
                });
            }
            oldTicket.BookingDate = ticketDto.BookingDate;
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
            var userTickets = await _context.UserTickets.Where(x => x.TicketId == oldTicket.Id).ToListAsync();
            if(userTickets.Count > 0)
            {
            _context.UserTickets.RemoveRange(userTickets);
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
