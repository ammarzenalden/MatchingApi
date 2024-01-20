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
        [HttpGet("GetAllTicket")]
        public async Task<ActionResult> GetAllTicket()
        {
            var pp = await _context.Tickets.Where(x => x.CreatorId == GetUserId()).ToListAsync();
            return Ok(new
            {
                success = true,
                data = pp
            });
        }
        [HttpGet("GetLastTicket")]
        public async Task<ActionResult> GetLastTicket()
        {
            var pp = await _context.Tickets.Where(x => x.CreatorId == GetUserId()).OrderBy(x => x.Id).ToListAsync();
            List<Ticket> ff = new();
            if (pp.Count > 0)
            {
                foreach (var item in pp)
                {
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
                    if (oldUserTicket == null)
                    {
                        return Ok(new
                        {
                            success = true,
                            data = item
                        });
                    }
                    else
                    {
                        continue;
                    }
                }
                return Ok(new
                {
                    success = true,
                    data = pp.Last()
                });
            }
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

            if (oldTicket.Count > 0)
            {
                foreach (var item in oldTicket)
                {
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
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
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
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
            Ticket ticket = new()
            {
                Type = ticketDto.Type,
                BookingDate = ticketDto.BookingDate,
                CreatorId = GetUserId(),
                RoomId = ticketDto.RoomId
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            if (ticket.Type!.ToLower() == "premium")
            {
                UserTicket userTicket = new()
                {
                    SenderId = GetUserId(),
                    ReceiverId = GetUserId(),
                    TicketId = ticket.Id,
                    TicketStatus = "Accepted"
                };
                _context.UserTickets.Add(userTicket);
                await _context.SaveChangesAsync();
            }
            var user = _context.Users.Find(GetUserId());
            var room = _context.Rooms.Find(ticket.RoomId);
            Email email = new();
            try
            {
                //change the body to html when the user has received new friend request 
                //for example : body = @"<html><body>....</body></html>";
                string body = $@"<html><body><div style=""width: 100%;padding: 30px 0px;background-color: #FFE2C1;"">
                                <div>
                                <h1 style=""text-align: center;font-size:1.8rem"">Confirmation of Your Love Lockdown Ticket Purchase</h1>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Dear {user!.Name}</p><br>
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Thank you for choosing Love Lockdown for your upcoming dating event! We're excited to have you join us.</p><br>      
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Your ticket purchase has been successfully processed, and here are the details:</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Event: Love Lockdown Dating Event</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Date and Time: {ticket.BookingDate}</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Venue: {room!.Location}</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Please keep this confirmation email for your records. We will reach out if there are any updates or additional information leading up to the event.</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">If you have any questions or need further assistance, feel free to contact us.</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">We look forward to creating memorable moments with you at Love Lockdown!</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px"">Best regards,</p><br> 
                                <p style=""text-align: center;font-size:1.1rem;padding:0px 10px""><a href='https://love-lockdown.vercel.app/'>Love Lockdown</a></p><br>
                                </div>
                                </div></body></html>";
                //change the subject to suitable one 
                //for example : subject = "new request";
                string subject = "Confirmation of Your Love Lockdown Ticket Purchase";
                email.SendEmail(user!.Email!, subject, body);

            }
            catch
            {

            }
            return Ok(new
            {
                success = true,
                data = ticket
            });
        }
        [HttpPut("UpdateTicket")]
        public async Task<ActionResult> UpdateTicket(TicketDto ticketDto)
        {
            var oldTicket = await _context.Tickets.Where(x => x.CreatorId == GetUserId()).ToListAsync();

            if (oldTicket.Count > 0)
            {
                foreach (var item in oldTicket)
                {
                    var oldUserTicket = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
                    if (oldUserTicket == null)
                    {
                        if (item.Type == "premium")
                        {
                            var userTicekt = _context.UserTickets.Where(x => x.TicketId == item.Id && x.TicketStatus == "Accepted").ToList();
                            if (userTicekt.Count > 0)
                            {
                                _context.UserTickets.RemoveRange(userTicekt);
                            }
                        }
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
                        item.BookingDate = ticketDto.BookingDate;
                        item.RoomId = ticketDto.RoomId;
                        item.Type = ticketDto.Type;
                        _context.Tickets.Update(item);
                        await _context.SaveChangesAsync();
                        return Ok(new
                        {
                            success = true,
                            data = oldTicket
                        });
                    }
                }
            }
            return NotFound(new
            {
                success = false,
                message = "you dont have ticket"

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
            if (userTickets.Count > 0)
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
            var userTickets = await _context.UserTickets.Where(x => x.SenderId == GetUserId()).ToListAsync();
            return Ok(new
            {
                success = true,
                data = userTickets
            });
        }
    }
}
