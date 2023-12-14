using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Matching.Model
{
    public class PersonalPreferences
    {
        [Key]
        public int Id { get; set; }
        public string? SexualPreference { get; set; }
        public string? CommitmentLevel { get; set; }
        public string[]? PersonalValues { get; set; }
        public string[]? PersonalBelieves { get; set; }
        public string? FavoriteHolidayDestination {  get; set; }
        public string[]? FreeTime {  get; set; }
        public string[]? MusicGenres { get; set; }
        public string? Pet { get; set; }
        public int UserId {  get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string[]? Languages { get; set; }
        public float Height { get; set; }
        public string? BodyType { get; set; }
    }
}
