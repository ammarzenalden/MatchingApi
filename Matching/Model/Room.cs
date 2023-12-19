using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public string? Description { get; set; }
        public int? CreatorId { get; set; }
        //[JsonIgnore]
        //[ForeignKey("CreatorId")]
        //public User? Creator { get; set; }
        public string? Lat {  get; set; }
        public string? Long { get; set; }
        public string? Location { get; set; }

    }
}
