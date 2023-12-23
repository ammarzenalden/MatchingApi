using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        [HttpGet("GetPersonalPreferences")]
        public async Task<ActionResult> GetPersonalPreferences()
        {
            var pp = await _context.PersonalPreferences.FirstOrDefaultAsync(x=> x.UserId == GetUserId());
            return Ok(new
            {
                success = true,
                data = pp
            });
        }
        [HttpGet("GetPotentialPartnerPreferences")]
        public async Task<ActionResult> GetPotentialPartnerPreferences()
        {
            var pp = await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            return Ok(new
            {
                success = true,
                data = pp
            });
        }
        [HttpPost("AddPersonalPreferences")]
        public async Task<ActionResult> AddPersonalPreferences(PersonalPreferencesDto personalPreferencesDto)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in personalPreferencesDto.GetType().GetProperties())
            {
                object value = property.GetValue(personalPreferencesDto)!;
                if (value == null)
                {
                    hasNull = true;
                    theNull = $"Property '{property.Name}' is null.";
                }
            }
            if (hasNull)
            {
                return BadRequest(new
                {
                    success = false,
                    message = theNull
                });
            };
            var oldOne = await _context.PersonalPreferences.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            if(oldOne != null)
            {
                return Conflict(new
                {
                    success = false,
                    message = "you already have one",
                    data = oldOne
                });
            }
            PersonalPreferences personalPreferences = new()
            {
                Age = personalPreferencesDto.Age,
                BodyType = personalPreferencesDto.BodyType,
                CommitmentLevel = personalPreferencesDto.CommitmentLevel,
                FavoriteHolidayDestination = personalPreferencesDto.FavoriteHolidayDestination,
                FreeTime = personalPreferencesDto.FreeTime,
                Gender = personalPreferencesDto.Gender,
                Height = personalPreferencesDto.Height,
                Languages = personalPreferencesDto.Languages,
                MusicGenres = personalPreferencesDto.MusicGenres,
                PersonalBelieves = personalPreferencesDto.PersonalBelieves,
                PersonalValues = personalPreferencesDto.PersonalValues,
                Pet = personalPreferencesDto.Pet,
                SexualPreference = personalPreferencesDto.SexualPreference,
                UserId = GetUserId()
            };
            _context.PersonalPreferences.Add(personalPreferences);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = personalPreferences
            });
        }
        [HttpPost("AddPotentialPartnerPreferences")]
        public async Task<ActionResult> AddPotentialPartnerPreferences(PotentialPartnerPreferencesDto potentialPartnerPreferencesDto)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in potentialPartnerPreferencesDto.GetType().GetProperties())
            {
                object value = property.GetValue(potentialPartnerPreferencesDto)!;
                if (value == null)
                {
                    hasNull = true;
                    theNull = $"Property '{property.Name}' is null.";
                }
            }
            if (hasNull)
            {
                return BadRequest(new
                {
                    success = false,
                    message = theNull
                });
            };
            var oldOne = await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            if (oldOne != null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "you already have one",
                    data = oldOne
                });
            }
            PotentialPartnerPreferences potentialPartnerPreferences = new()
            {
                BodyType = potentialPartnerPreferencesDto.BodyType,
                CommitmentLevel = potentialPartnerPreferencesDto.CommitmentLevel,
                FavoriteHolidayDestination = potentialPartnerPreferencesDto.FavoriteHolidayDestination,
                FreeTime = potentialPartnerPreferencesDto.FreeTime,
                Gender = potentialPartnerPreferencesDto.Gender,
                MusicGenres = potentialPartnerPreferencesDto.MusicGenres,
                PersonalBelieves = potentialPartnerPreferencesDto.PersonalBelieves,
                Pet = potentialPartnerPreferencesDto.Pet,
                UserId = GetUserId(),
                MaxAge = potentialPartnerPreferencesDto.MaxAge,
                MinAge = potentialPartnerPreferencesDto.MinAge
            };
            _context.PotentialPartnerPreferences.Add(potentialPartnerPreferences);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = potentialPartnerPreferences
            });
        }
        [HttpPut("UpdatePersonalPreferences")]
        public async Task<ActionResult> UpdatePersonalPreferences(PersonalPreferencesDto personalPreferencesDto)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in personalPreferencesDto.GetType().GetProperties())
            {
                object value = property.GetValue(personalPreferencesDto)!;
                if (value == null)
                {
                    hasNull = true;
                    theNull = $"Property '{property.Name}' is null.";
                }
            }
            if (hasNull)
            {
                return BadRequest(new
                {
                    success = false,
                    message = theNull
                });
            };
            var oldOne = await _context.PersonalPreferences.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            oldOne!.MusicGenres = personalPreferencesDto.MusicGenres;
            oldOne.Pet = personalPreferencesDto.Pet;
            oldOne.PersonalBelieves = personalPreferencesDto.PersonalBelieves;
            oldOne.PersonalValues = personalPreferencesDto.PersonalValues;
            oldOne.SexualPreference = personalPreferencesDto.SexualPreference;
            oldOne.Gender = personalPreferencesDto.Gender;
            oldOne.Age = personalPreferencesDto.Age;
            oldOne.BodyType = personalPreferencesDto.BodyType;
            oldOne.CommitmentLevel = personalPreferencesDto.CommitmentLevel;
            oldOne.FavoriteHolidayDestination = personalPreferencesDto.FavoriteHolidayDestination;
            oldOne.FreeTime = personalPreferencesDto.FreeTime;
            oldOne.Height = personalPreferencesDto.Height;
            oldOne.Languages = personalPreferencesDto.Languages;
            _context.PersonalPreferences.Update(oldOne);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = oldOne
            });
        }
        [HttpPut("UpdatePotentialPartnerPreferences")]
        public async Task<ActionResult> UpdatePotentialPartnerPreferences(PotentialPartnerPreferencesDto potentialPartnerPreferencesDto)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in potentialPartnerPreferencesDto.GetType().GetProperties())
            {
                object value = property.GetValue(potentialPartnerPreferencesDto)!;
                if (value == null)
                {
                    hasNull = true;
                    theNull = $"Property '{property.Name}' is null.";
                }
            }
            if (hasNull)
            {
                return BadRequest(new
                {
                    success = false,
                    message = theNull
                });
            };
            var oldOne = await _context.PotentialPartnerPreferences.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            oldOne!.MusicGenres = potentialPartnerPreferencesDto.MusicGenres;
            oldOne.Pet = potentialPartnerPreferencesDto.Pet;
            oldOne.PersonalBelieves = potentialPartnerPreferencesDto.PersonalBelieves;
            oldOne.Gender = potentialPartnerPreferencesDto.Gender;
            oldOne.BodyType = potentialPartnerPreferencesDto.BodyType;
            oldOne.CommitmentLevel = potentialPartnerPreferencesDto.CommitmentLevel;
            oldOne.FavoriteHolidayDestination = potentialPartnerPreferencesDto.FavoriteHolidayDestination;
            oldOne.FreeTime = potentialPartnerPreferencesDto.FreeTime;
            oldOne.MinAge = potentialPartnerPreferencesDto.MinAge;
            oldOne.MaxAge = potentialPartnerPreferencesDto.MaxAge;
            _context.PotentialPartnerPreferences.Update(oldOne);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = oldOne
            });
        }
    }
}
