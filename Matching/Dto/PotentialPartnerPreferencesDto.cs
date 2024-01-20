namespace Matching.Dto
{
    public class PotentialPartnerPreferencesDto
    {
        public string[]? Gender { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string? BodyType { get; set; }
        public string? CommitmentLevel { get; set; }
        public string[]? PersonalBelieves { get; set; }
        public string? FavoriteHolidayDestination { get; set; }
        public string[]? FreeTime { get; set; }
        public string[]? MusicGenres { get; set; }
        public string? Pet { get; set; }
    }
}
