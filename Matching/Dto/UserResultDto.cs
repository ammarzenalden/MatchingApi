using System.ComponentModel.DataAnnotations;

namespace Matching.Dto
{
    public class UserResultDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }

    }
}
