using Matching.Configure;
using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(ApplicationDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }

        [HttpPost("Upsert")]
        [AllowAnonymous]
        public async Task<IActionResult> Upsert([FromForm]UserRegisterDto userDto, IFormFile? image)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in userDto.GetType().GetProperties())
            {
                object value = property.GetValue(userDto)!;
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
            var currentUser = _context.Users.FirstOrDefault(x => x.Email!.ToLower() == userDto.Email!.ToLower());
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (currentUser is null)
            {
                User user = new()
                {
                    Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                    Name = userDto.Name,
                    PhoneNumber = userDto.PhoneNumber,
                    Email = userDto.Email
                };
                
                if (image is null)
                {
                    user.ImageUrl = null;
                }
                else
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\user");
                    using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
                    {
                        image.CopyTo(imageStream);
                    }
                    user.ImageUrl = wwwRootPath + @"\images\product\" + imageName;
                } 
            
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    data = user.ToJson()
                });
            }
            else
            {
                if(currentUser.Id != GetUserId())
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "you are not the owner"
                    });
                }
                else
                {
                    currentUser.PhoneNumber = userDto.PhoneNumber;
                    currentUser.Email = userDto.Email;
                    currentUser.Name = userDto.Name;
                    currentUser.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                    
                    if (currentUser.ImageUrl is not null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, currentUser.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }
                    if (image != null)
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string imagePath = Path.Combine(wwwRootPath, @"images\product");
                        using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
                        {
                            image.CopyTo(imageStream);
                        }
                        currentUser.ImageUrl = wwwRootPath + @"\images\product\" + imageName;
                    }
                    return Ok(new
                    {
                        success = true,
                        data = currentUser.ToJson()
                    });
                }
            }
            
        }
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _context.Users.FindAsync(GetUserId());
            
            return Ok(new
            {
                success = true,
                data = user!.ToJson()
            });

        }
        [HttpPost("LogIn")]
        [AllowAnonymous]
        public ActionResult LogIn(UserDto user)
        {
            var currentUser = _context.Users.FirstOrDefault(x => x.Email!.ToLower() == user.Email!.ToLower());
            if (currentUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password))
            {
                return Unauthorized(new { success = false, message = "wrong email or password" });
            }

            GenerateToken g = new GenerateToken(_context, _config);
            string theToken = g.GenerateApiToken(currentUser.Id);


            return Ok(new
            {
                success = true,
                message = "success",
                data = new
                {
                    token = theToken,
                    user = currentUser.ToJson()
                }
            });


        }
    }
}
