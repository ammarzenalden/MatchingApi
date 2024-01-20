using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class RoomImages
    {
        [Key]
        public int Id { get; set; }
        public int? RoomId { get; set; }
        [JsonIgnore]
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
        public string? image {  get; set; }
    }
}
