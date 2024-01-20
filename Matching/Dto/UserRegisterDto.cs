using System.ComponentModel.DataAnnotations;

namespace Matching.Dto
{
    public class UserRegisterDto
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

    }
}
