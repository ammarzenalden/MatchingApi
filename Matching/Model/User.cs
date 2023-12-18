using System.ComponentModel.DataAnnotations;

namespace Matching.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Length(3, 25)]
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? ImageUrl { get; set; }
        public string? Role { get; set; }
        public object ToJson()
        {
            return new { Id = Id, Name = Name, Email = Email, PhoneNumber = PhoneNumber,ImageUrl = ImageUrl};
        }
    }
}
