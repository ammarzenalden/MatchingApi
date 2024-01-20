namespace Matching.Configure
{
    public class TheMatching
    {

        public double CalculateSimilarity(string[] user1, string[] user2)
        {
            int matchingCategories = user1.Count(pref => user2.Contains(pref));
            double similarityScore = (double)matchingCategories / user1.Length;
            return similarityScore;
        }

        public List<Tuple<string[], double>> FindSuitablePartners(string[] user, List<string[]> potentialPartners)
        {
            var scores = new List<Tuple<string[], double>>();

            // Parallelize the loop for improved performance
            Parallel.ForEach(potentialPartners, partner =>
            {
                double similarityScore = CalculateSimilarity(user, partner);
                scores.Add(new Tuple<string[], double>(partner, similarityScore));
            });

            // Order the list of partners based on similarity scores (highest to lowest)
            var sortedPartners = scores.OrderByDescending(x => x.Item2).ToList();

            return sortedPartners;
        }

        
    }
}
