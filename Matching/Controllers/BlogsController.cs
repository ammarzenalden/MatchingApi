using Matching.Data;
using Matching.Dto;
using Matching.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Matching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public BlogsController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        private int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
        [HttpPost("CreateBlog")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateBlog([FromForm] BlogDto blogDto, IFormFile? image)
        {
            
            Blog blog = new()
            {
                Description = blogDto.Description,
                Title = blogDto.Title
            };
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (image is null)
            {
                blog.Image = null;
            }
            else
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string imagePath = Path.Combine(wwwRootPath, @"images\blog");
                using (var imageStream = new FileStream(Path.Combine(imagePath, imageName), FileMode.Create))
                {
                    image.CopyTo(imageStream);
                }
                //blog.Image = wwwRootPath + @"\images\blog\" + imageName;
                blog.Image = $"{Request.Scheme}://{Request.Host}/images/blog/{imageName}";
            }
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                data = blog
            });
        }
        [HttpGet("GetBlogs")]
        [AllowAnonymous]
        public async Task<ActionResult> GetBlogs()
        {
            var blogs = await _context.Blogs.ToListAsync();
            return Ok(new
            {
                success = true,
                data = blogs
            });
        }
            
    }
}
