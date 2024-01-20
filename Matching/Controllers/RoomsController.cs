using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("GetRooms")]
        [AllowAnonymous]
        public async Task<ActionResult> GetRooms()
        {
            List<RoomResultDto> roomResults = new();
            var rooms = await _context.Rooms.ToListAsync();
            foreach(var room in rooms)
            {
                RoomResultDto roomResultDto = new();
                var images = await _context.RoomImages.Where(x=>x.RoomId ==  room.Id).ToListAsync();
                roomResultDto.Room = room;
                roomResultDto.images = new List<string>();
                if (images.Count > 0)
                {
                    foreach (var img in images)
                    {
                        roomResultDto.images.Add(img.image!);
                    }
                }
                else
                {
                    roomResultDto.images = null;
                }
                roomResults.Add(roomResultDto);
            }
            return Ok(new
            {
                success = true,
                data = roomResults
            });
            
        }
        [HttpPost("CreateRoom")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateRoom([FromForm]RoomDto roomDto, List<IFormFile>? images)
        {
            Boolean hasNull = false;
            string theNull = "";
            foreach (PropertyInfo property in roomDto.GetType().GetProperties())
            {
                object value = property.GetValue(roomDto)!;
                if(property.Name == "Long" || property.Name == "Lat")
                {
                    continue;
                }
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
                //CreatorId = GetUserId(),
                RoomName = roomDto.RoomName,
                Lat = roomDto.Lat,
                Long = roomDto.Long,
                Location = roomDto.Location
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();
            List<RoomImages> imgs = new();
            foreach (var image in images!) {
            
                
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\room");
                    using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
                    {
                        image.CopyTo(imageStream);
                    }
                RoomImages roomImages = new RoomImages();
                roomImages.RoomId = room.Id;
                //roomImages.image = wwwRootPath + @"\images\room\" + imageName;
                roomImages.image = $"{Request.Scheme}://{Request.Host}/images/room/{imageName}";
                _context.RoomImages.Add(roomImages);
                imgs.Add(roomImages);

            }
            
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = room,
                images = imgs
            });
        }
        //[HttpPut("UpdateRoom/{id}")]
        //public async Task<ActionResult> UpdateRoom([FromForm]RoomDto roomDto, List<IFormFile>? images,int id)
        //{
        //    var oldRoom = await _context.Rooms.FindAsync(id);

        //    if(oldRoom == null)
        //    {
        //        return NotFound(new
        //        {
        //            success = false,
        //            message = "there is no Room By this Id"
        //        });
        //    }
        //    if(oldRoom.CreatorId != GetUserId())
        //    {
        //        return Unauthorized(new
        //        {
        //            success = false,
        //            message = "you are not the owner"
        //        });
        //    }
        //    string wwwRootPath = _webHostEnvironment.WebRootPath;
        //    oldRoom.Description = roomDto.Description;
        //    oldRoom.RoomName = roomDto.RoomName;
        //    oldRoom.Lat = roomDto.Lat;
        //    oldRoom.Long = roomDto.Long;
        //    oldRoom.Location = roomDto.Location;
        //    var imageUrl = await _context.RoomImages.Where(x => x.RoomId == oldRoom.Id).ToListAsync();
        //    if (imageUrl.Count != 0)
        //    {
        //        foreach(var img in imageUrl)
        //        {
        //            var oldImagePath = Path.Combine(wwwRootPath, img.image!.TrimStart('\\'));
                    
        //            if (System.IO.File.Exists(oldImagePath))
        //            {
        //                System.IO.File.Delete(oldImagePath);
        //            }
        //        }
        //    }if(images is not null)
        //    {
        //        foreach (var image in images)
        //        {

        //            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        //            string imagePath = Path.Combine(wwwRootPath, @"images\room");
        //            using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
        //            {
        //                image.CopyTo(imageStream);
        //            }

        //            RoomImages roomImages = new RoomImages();
        //            roomImages.RoomId = oldRoom.Id;
        //            roomImages.image = wwwRootPath + @"\images\room\" + imageName;
        //            _context.RoomImages.Add(roomImages);
        //        }
        //    }
        //    _context.Rooms.Update(oldRoom);
        //    await _context.SaveChangesAsync();
        //    return Ok(new
        //    {
        //        success = true,
        //        data = oldRoom
        //    });
        //}
        //[HttpDelete("DeleteRoom/{id}")]
        //public async Task<ActionResult> DeleteRoom(int id)
        //{
        //    var oldRoom = await _context.Rooms.FindAsync(id);

        //    if (oldRoom == null)
        //    {
        //        return NotFound(new
        //        {
        //            success = false,
        //            message = "there is no Room By this Id"
        //        });
        //    }
        //    if(oldRoom.CreatorId != GetUserId())
        //    {
        //        return Unauthorized(new
        //        {
        //            success = false,
        //            message = "you are not the owner"
        //        });
        //    }
        //    var roomImages = await _context.RoomImages.Where(x => x.RoomId == oldRoom.Id).ToListAsync();
        //    if(roomImages.Count != 0)
        //    {
        //        foreach(var img in roomImages)
        //        {
        //            _context.RoomImages.Remove(img);
        //        }
        //    }
        //    _context.Rooms.Remove(oldRoom);
        //    await _context.SaveChangesAsync();
        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Deleted"
        //    });
        //}
    }
}
