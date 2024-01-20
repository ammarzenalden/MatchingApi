using Matching.Configure;
using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.EntityFrameworkCore;

namespace matching.Configure
    {
        public class TheMatchingProcess
        {
            private readonly ApplicationDbContext _context;
            public TheMatchingProcess(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<MatchingResultDto>> MatchingProcess(PersonalPreferences currentPersonalPref, PotentialPartnerPreferences currentPotentialPartnerPref,
                List<PersonalPreferences> allPersonalPref, List<PotentialPartnerPreferences> allPotentialPartnerPref, int userId)
            {

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
                        if (allpers.UserId == subpers.UserId)
                        {
                            var userTicket = await _context.UserTickets.Where(x => x.SenderId == userId
                             && x.ReceiverId == subpers.UserId && x.TicketStatus == "Rejected").ToListAsync();
                            if (userTicket.Count > 0)
                            {
                                break;
                            }
                            if (currentPotentialPartnerPref.Gender!.Contains(allpers.Gender) &&
                                currentPotentialPartnerPref.MinAge < allpers.Age &&
                                allpers.Age < currentPotentialPartnerPref.MaxAge &&
                                subpers.Gender!.Contains(currentPersonalPref.Gender) &&
                                subpers.MinAge < currentPersonalPref.Age &&
                                currentPersonalPref.Age < subpers.MaxAge)
                            {

                                string[] pers =
                                {
                                allpers.Pet!,
                                allpers.CommitmentLevel!,
                                allpers.FavoriteHolidayDestination!,

                            };
                                if (currentPotentialPartnerPref.BodyType! != "any_preference")
                                {
                                    Array.Resize(ref pers, pers.Length + 1);
                                    pers[pers.Length - 1] = allpers.BodyType!;
                                }
                                pers = pers.Concat(allpers.MusicGenres!).ToArray();
                                pers = pers.Concat(allpers.PersonalBelieves!).ToArray();
                                pers = pers.Concat(allpers.FreeTime!).ToArray();

                                string[] possiblePers = pers;
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
                return sortedPartners;
            }
        }
    }
