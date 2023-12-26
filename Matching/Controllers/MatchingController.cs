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
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == userId);
            if(ticket == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });
            }
            if(ticket.Type!.ToLower() != "smart")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "the type of your ticket must be smart ticket"
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
            
            string[] theUserPersonal =
            {
                currentPersonalPref.Pet!,
                currentPersonalPref.CommitmentLevel!,
                currentPersonalPref.FavoriteHolidayDestination!,
                

            };
            theUserPersonal = theUserPersonal.Concat(currentPersonalPref.MusicGenres!).ToArray();
            theUserPersonal = theUserPersonal.Concat(currentPersonalPref.PersonalBelieves!).ToArray();
            theUserPersonal = theUserPersonal.Concat(currentPersonalPref.FreeTime!).ToArray();
            Array.Resize(ref theUserPersonal, theUserPersonal.Length + 1);
            theUserPersonal[theUserPersonal.Length - 1] = currentPersonalPref.BodyType!;

            string[] theUserPotential =
            {
                currentPotentialPartnerPref.Pet!,
                currentPotentialPartnerPref.CommitmentLevel!,
                currentPotentialPartnerPref.FavoriteHolidayDestination!,
            };
            theUserPotential = theUserPotential.Concat(currentPotentialPartnerPref.MusicGenres!).ToArray();
            theUserPotential = theUserPotential.Concat(currentPotentialPartnerPref.PersonalBelieves!).ToArray();
            theUserPotential = theUserPotential.Concat(currentPotentialPartnerPref.FreeTime!).ToArray();
            theUserPotential = theUserPotential.Concat(currentPotentialPartnerPref.FreeTime!).ToArray();
            if (currentPotentialPartnerPref.BodyType! != "any_preference")
            {
                Array.Resize(ref theUserPotential, theUserPotential.Length + 1);
                theUserPotential[theUserPotential.Length - 1] = currentPotentialPartnerPref.BodyType!;
            }
            
            TheMatching matching = new();
            var lastResult = new List<MatchingResultDto>();
            foreach (var allpers in allPersonalPref)
            {
                foreach (var subpers in allPotentialPartnerPref)
                {
                    if(allpers.UserId == subpers.UserId)
                    {
                        var userTicket = await _context.UserTickets.Where(x => x.SenderId == GetUserId()
                         && x.ReceiverId == subpers.UserId && x.TicketStatus == "Rejected").ToListAsync();
                        if(userTicket.Count > 0)
                        {
                            break;
                        }
                        if (currentPotentialPartnerPref.Gender!.Contains(allpers.Gender) &&
                            currentPotentialPartnerPref.MinAge < allpers.Age &&
                            allpers.Age < currentPotentialPartnerPref.MaxAge &&
                            /*currentPotentialPartnerPref.BodyType!.Contains(allpers.BodyType)&&*/
                            subpers.Gender!.Contains(currentPersonalPref.Gender) &&
                            subpers.MinAge < currentPersonalPref.Age &&
                            currentPersonalPref.Age < subpers.MaxAge /*&&
                            subpers.BodyType!.Contains(currentPersonalPref.BodyType)*/)
                        {
                            
                            string[] pers =
                            {
                                allpers.Pet!,
                                allpers.CommitmentLevel!,
                                allpers.FavoriteHolidayDestination!,
                                //allpers.BodyType

                            };
                            if (currentPotentialPartnerPref.BodyType! != "any_preference")
                            {
                                Array.Resize(ref pers, pers.Length + 1);
                                pers[pers.Length - 1] = allpers.BodyType!;
                            }
                            pers = pers.Concat(allpers.MusicGenres!).ToArray();
                            pers = pers.Concat(allpers.PersonalBelieves!).ToArray();
                            pers = pers.Concat(allpers.FreeTime!).ToArray();

                            string[]possiblePers =pers;
                            string[] spers =
                            {
                                subpers.Pet!,
                                subpers.CommitmentLevel!,
                                subpers.FavoriteHolidayDestination!,

                            };

                            spers = spers.Concat(subpers.MusicGenres!).ToArray();
                            spers = spers.Concat(subpers.PersonalBelieves!).ToArray();
                            spers = spers.Concat(subpers.FreeTime!).ToArray();
                            if (subpers.BodyType! != "any_preference")
                            {
                                Array.Resize(ref spers, spers.Length + 1);
                                spers[spers.Length - 1] = subpers.BodyType!;

                            }
                            else
                            {
                                theUserPersonal = theUserPersonal.Where(w => w != theUserPersonal[theUserPersonal.Length - 1]).ToArray();
                            }
                            //spers = spers.Concat(subpers.BodyType!).ToArray();
                            string[] possiblePot = spers;
                            double first = matching.CalculateSimilarity(theUserPersonal, possiblePot);
                            double second = matching.CalculateSimilarity(theUserPotential, possiblePers);

                            double ava = (first + second) / 2;
                            var user = await _context.Users.FindAsync(allpers.UserId);
                            
                            PersonalPreferences pr = await _context.PersonalPreferences.FirstOrDefaultAsync(x => x.UserId == allpers.UserId);
                            PotentialPartnerPreferences po = await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId == allpers.UserId);
                            UserResultDto theUser = new()
                            {
                                Id = user!.Id,
                                Email = user!.Email,
                                ImageUrl = user.ImageUrl,
                                Name = user.Name,
                                PhoneNumber = user.PhoneNumber
                            };
                            lastResult.Add(new MatchingResultDto
                            {
                                User = theUser,
                                SimilarityScore = ava,
                                PersonalPreferences = pr!,
                                PotentialPartnerPreferences = po!
                            });
                        }
                    }
                }
            }
            var sortedPartners = lastResult.OrderByDescending(x => x.SimilarityScore).Take(10).ToList();
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
                            TicketId = ticket.Id,
                            TicketStatus = "pending"
                        };
                        _context.UserTickets.Add(userTicket);
                    }
                    //UserTicket userTicket = new()
                    //{
                    //    SenderId = userId,
                    //    ReceiverId = ss.User!.Id,
                    //    TicketId = ticket!.Id,
                    //    TicketStatus = "pending"
                    //};
                    //_context.UserTickets.Add(userTicket);
                }
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    data = sortedPartners
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    data = sortedPartners
                });
            }
        }
        //[HttpPost("ChosePartner/{id}")]
        //public async Task<ActionResult> ChosePartner(int id)
        //{
        //    var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId());
        //    if(ticket == null)
        //    {
        //        return NotFound(new
        //        {
        //            success = false,
        //            message = "you do not have a ticket"
        //        });
        //    }
        //    if(ticket.Type != "premium")
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = "the type of your ticket must be premium ticket"
        //        });
        //    }
        //    var user = await _context.Users.FindAsync(id);
        //    if(user == null)
        //    {
        //        return NotFound(new
        //        {
        //            success = false,
        //            message = "there is no user by this id"
        //        });
        //    }
        //    UserTicket userTicket = new()
        //    {
        //        ReceiverId = id,
        //        SenderId = GetUserId(),
        //        TicketId = ticket.Id,
        //        TicketStatus = "Accepted",
        //    };
        //    _context.UserTickets.Add(userTicket);
        //    await _context.SaveChangesAsync();
        //    return Ok(new
        //    {
        //        success = true,
        //        data = new
        //        {
        //            userTicket = userTicket,
        //            ticket = ticket
        //        }
        //    });
        //}
        [HttpGet("RandomPartner")]
        public async Task<ActionResult> RandomPartner()
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatorId == GetUserId());
            if (ticket == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you do not have a ticket"
                });
            }
            if (ticket.Type!.ToLower() != "regular")
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
            if(users.Count > 0)
            {
                foreach(var uss in users)
                {
                    var usTicket = await _context.UserTickets.Where(x => x.SenderId == GetUserId()
                         && x.ReceiverId == uss!.Id && x.TicketStatus == "Rejected").ToListAsync();
                    if (usTicket.Count > 0)
                    {
                        continue;
                    }
                    var similrTicekt = await _context.Tickets
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefaultAsync(x => x.CreatorId != GetUserId() && x.Type!.ToLower() == ticket.Type.ToLower());
                    if (similrTicekt == null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "no have the same Type of ticket"
                        });
                    }
                    var similrUser = await _context.Users.FindAsync(similrTicekt.CreatorId);
                    int count = 0;
                    if (oldUserTicket.Count > 0)
                    {
                        foreach (var ust in oldUserTicket)
                        {
                            if (ust.ReceiverId == similrUser!.Id)
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
                            TicketId = ticket.Id,
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
            List<string> a = new();
            return Ok(new
            {
                success = true,
                data = a
            });
        }
    }
}
