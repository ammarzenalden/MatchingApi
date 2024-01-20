using Matching.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Dto
{
    public class TicketDto
    {
        public string? BookingDate { get; set; }
        public int? RoomId { get; set; }
        public string? Type { get; set; }
    }
}
