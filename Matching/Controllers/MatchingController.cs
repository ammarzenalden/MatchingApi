using matching.Configure;
using Matching.Configure;
using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ExceptionServices;
using System.Security.Claims;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MatchingController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        
        [HttpGet("GetMatchingUsers")]
        public async Task<ActionResult> GetMatchingUsers()
        {
            int userId = GetUserId();
            var ticket = await _context.Tickets.Where(x => x.CreatorId == userId).ToListAsync();
            if(ticket == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });
            }
            Ticket ticket1 = new();
            int ucount = 0;
            foreach (var item in ticket)
            {
                var oldUserTickets = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
                if (oldUserTickets == null)
                {
                    ucount += 1;
                    ticket1 = item;
                    if (item.Type!.ToLower() != "smart")
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "the type of your ticket must be smart ticket"
                        });
                    }
                }
            }
            if (ucount == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });

            }
            var currentUser = await _context.Users.FindAsync(userId);
            var oldUserTicket = await _context.UserTickets.Where(x => x.SenderId == GetUserId()).ToListAsync();
            if(oldUserTicket.Count > 0)
            {
                foreach(var ust in oldUserTicket)
                {
                    if(ust.TicketStatus!.ToLower() == "accepted")
                    {
                        var matchingPerson = await _context.Users.FindAsync(ust.ReceiverId);
                        var userPref  =await _context.PersonalPreferences.FirstOrDefaultAsync(x=>x.UserId==ust.ReceiverId);
                        var userPote = await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId == ust.ReceiverId);

                        UserResultDto resultDto = new()
                        {
                            Email = matchingPerson!.Email,
                            Id = matchingPerson.Id,
                            ImageUrl = matchingPerson.ImageUrl,
                            Name = matchingPerson.Name,
                            PhoneNumber = matchingPerson.PhoneNumber
                        };
                        MatchingResultDto matchingResultDto = new()
                        {
                            PersonalPreferences = userPref,
                            PotentialPartnerPreferences = userPote,
                            User = resultDto,
                            SimilarityScore = 0
                        };
                        return Ok(new
                        {
                            success = true,
                            message = "you have already find a matching person",
                            data = matchingResultDto
                        });
                    }
                }
            }
            var currentPersonalPref =await _context.PersonalPreferences.FirstOrDefaultAsync(x => x.UserId == userId);
            var currentPotentialPartnerPref =await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId==userId);
            if (currentPersonalPref == null || currentPotentialPartnerPref==null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "this user didnt fill the forms"
                });
            }
            var allPersonalPref =await _context.PersonalPreferences.Where(x => x.UserId != userId).ToListAsync();
            var allPotentialPartnerPref = await _context.PotentialPartnerPreferences
                .Where(x => x.UserId != userId)
                .ToListAsync();

            if (allPotentialPartnerPref == null)
            {
                return Ok(new
                {
                    success = true,
                    message = "no one have the same interest"
                });
            }
            TheMatchingProcess theMatchingProcess = new TheMatchingProcess(_context);
            var sortedPartners = await theMatchingProcess.MatchingProcess(currentPersonalPref, currentPotentialPartnerPref, allPersonalPref, allPotentialPartnerPref, userId);
            int count = 0;
            if(sortedPartners.Count > 0)
            {
                foreach(var ss in sortedPartners)
                {
                    if (oldUserTicket.Count > 0)
                    {
                        foreach (var ust in oldUserTicket)
                        {
                            if (ust.ReceiverId == ss.User!.Id)
                            {
                                count += 1;
                            }
                        }
                    }
                    if (count == 0)
                    {
                        UserTicket userTicket = new()
                        {
                            SenderId = GetUserId(),
                            ReceiverId = ss.User!.Id,
                            TicketId = ticket1.Id,
                            TicketStatus = "pending"
                        };
                        _context.UserTickets.Add(userTicket);
                    }
                }
                await _context.SaveChangesAsync();
            }
                return Ok(new
                {
                    success = true,
                    data = sortedPartners
                });
        }
        [HttpGet("RandomPartner")]
        public async Task<ActionResult> RandomPartner()
        {
            var ticket = await _context.Tickets.Where(x => x.CreatorId == GetUserId()).ToListAsync();
            if (ticket.Count == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });
            }
            
            int ucount = 0;
            foreach (var item in ticket)
            {
                var oldUserTickets = await _context.UserTickets.FirstOrDefaultAsync(x => x.TicketId == item.Id && (x.TicketStatus == "done" || x.TicketStatus == "cancelled"));
                if (oldUserTickets == null)
                {
                    ucount += 1;
                    if (item.Type!.ToLower() != "regular")
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "the type of your ticket must be Regular ticket"
                        });
                    }
                    var oldUserTicket = await _context.UserTickets.Where(x => x.SenderId == GetUserId()).ToListAsync();
                    if (oldUserTicket.Count > 0)
                    {
                        foreach (var ust in oldUserTicket)
                        {
                            if (ust.TicketStatus!.ToLower() == "accepted")
                            {
                                var matchingPerson = await _context.Users.FindAsync(ust.ReceiverId);
                                UserResultDto resultDto = new()
                                {
                                    Email = matchingPerson!.Email,
                                    Id = matchingPerson.Id,
                                    ImageUrl = matchingPerson.ImageUrl,
                                    Name = matchingPerson.Name,
                                    PhoneNumber = matchingPerson.PhoneNumber
                                };
                                return Ok(new
                                {
                                    success = true,
                                    message = "you have already find a matching person",
                                    data = resultDto
                                });
                            }
                        }
                    }
                    var users = await _context.Users.Where(x => x.Id != GetUserId()).ToListAsync();
                    if (users.Count > 0)
                    {
                            var similrTicekt = await _context.Tickets
                            .OrderBy(x => Guid.NewGuid())
                            .FirstOrDefaultAsync(x => x.CreatorId != GetUserId() && x.Type!.ToLower() == item.Type!.ToLower());
                            if (similrTicekt == null)
                            {
                                return Ok(new
                                {
                                    success = true,
                                    message = "no one have the same Type of ticket"
                                });
                            }
                            var ssTicekt = await _context.UserTickets.FirstOrDefaultAsync(x => x.SenderId == GetUserId()
                                 && x.ReceiverId == similrTicekt.CreatorId && x.TicketStatus!.ToLower() == "Rejected".ToLower());
                            if (ssTicekt == null)
                            {
                                var similrUser = await _context.Users.FindAsync(similrTicekt.CreatorId);
                                int count = 0;
                                if (oldUserTicket.Count > 0)
                                {
                                    foreach (var ust in oldUserTicket)
                                    {
                                        if (ust.ReceiverId == similrUser!.Id )
                                        {
                                            count += 1;
                                        }
                                    }

                                }
                                if (count == 0)
                                {
                                    UserTicket userTicket = new()
                                    {
                                        SenderId = GetUserId(),
                                        ReceiverId = similrUser!.Id,
                                        TicketId = item.Id,
                                        TicketStatus = "pending"
                                    };
                                    _context.UserTickets.Add(userTicket);
                                }
                                _context.SaveChanges();
                                UserResultDto theUser = new()
                                {
                                    Id = similrUser!.Id,
                                    Email = similrUser!.Email,
                                    ImageUrl = similrUser.ImageUrl,
                                    Name = similrUser.Name,
                                    PhoneNumber = similrUser.PhoneNumber
                                };
                                return Ok(new
                                {
                                    success = true,
                                    data = theUser
                                });
                            }
                    }
                }
            }
            if(ucount == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });
            }
            List<string> a = new();
            return Ok(new
            {
                success = true,
                data = a
            });
        }
    }
}
