using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        [JsonIgnore]
        [ForeignKey("SenderId")]
        public User? Sender { get; set; }
        [JsonIgnore]
        [ForeignKey("ReceiverId")]
        public User? Receiver { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
