using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public int? CreatorId {  get; set; }
        [JsonIgnore]
        [ForeignKey("CreatorId")]
        public User? Creator {  get; set; }
        public string? BookingDate { get; set; }
        public int? RoomId { get; set; }
        [JsonIgnore]
        [ForeignKey("RoomId")]
        public Room? Room { get; set; } 
        public string? Type {  get; set; }
    }
}
