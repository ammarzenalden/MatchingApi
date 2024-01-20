using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class UserTicket
    {
        [Key]
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId {  get; set; }
        public int? TicketId { get; set; }
        public string? TicketStatus { get; set; }
        [JsonIgnore]
        [ForeignKey("SenderId")]
        public User? Sender { get; set; }
        [JsonIgnore]
        [ForeignKey("ReceiverId")]
        public User? Receiver { get; set; }
        [JsonIgnore]
        [ForeignKey("TicketId")]
        public Ticket? Ticket { get; set; }

    }
}
