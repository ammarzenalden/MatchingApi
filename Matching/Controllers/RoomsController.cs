using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public RoomsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        [HttpPost("CreateRoom")]
        public async Task<ActionResult> CreateRoom([FromForm]RoomDto roomDto, IFormFile? image)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in roomDto.GetType().GetProperties())
            {
                object value = property.GetValue(roomDto)!;
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
            Room room = new()
            {
                Description = roomDto.Description,
                CreatorId = GetUserId(),
                RoomName = roomDto.RoomName
            };
            if (image is null)
            {
                room.ImageUrl = null;
            }
            else
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string imagePath = Path.Combine(wwwRootPath, @"images\user");
                using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
                {
                    image.CopyTo(imageStream);
                }
                room.ImageUrl = wwwRootPath + @"\images\product\" + imageName;
            }
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = room
            });
        }
        [HttpPut("UpdateRoom/{id}")]
        public async Task<ActionResult> UpdateRoom([FromForm]RoomDto roomDto,IFormFile? image,int id)
        {
            var oldRoom = await _context.Rooms.FindAsync(id);

            if(oldRoom == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "there is no Room By this Id"
                });
            }
            if(oldRoom.CreatorId != GetUserId())
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "you are not the owner"
                });
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            oldRoom.Description = roomDto.Description;
            oldRoom.RoomName = roomDto.RoomName;
            if (oldRoom.ImageUrl is not null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, oldRoom.ImageUrl.TrimStart('\\'));
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
                oldRoom.ImageUrl = wwwRootPath + @"\images\product\" + imageName;
            }
            _context.Rooms.Update(oldRoom);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = oldRoom
            });
        }
        [HttpDelete("DeleteRoom/{id}")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            var oldRoom = await _context.Rooms.FindAsync(id);

            if (oldRoom == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "there is no Room By this Id"
                });
            }
            if(oldRoom.CreatorId != GetUserId())
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "you are not the owner"
                });
            }
            _context.Rooms.Remove(oldRoom);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Deleted"
            });
        }
    }
}
