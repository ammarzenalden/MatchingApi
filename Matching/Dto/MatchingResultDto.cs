using Matching.Model;

namespace Matching.Dto
{
    public class MatchingResultDto
    {
        public User? User { get; set; }
        public double SimilarityScore { get; set; }
        public PersonalPreferences? PersonalPreferences { get; set; }
        public PotentialPartnerPreferences? PotentialPartnerPreferences { get; set; }

    }
}
