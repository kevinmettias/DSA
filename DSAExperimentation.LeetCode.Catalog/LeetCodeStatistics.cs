namespace DSAExperimentation.LeetCode.Catalog;

// The numbers that move every day, kept apart from the question so that a refresh which only
// moves them rewrites one file (Snapshot/Statistics.json) rather than every question's.
/// <summary>How LeetCode's users have fared on a question, as of the catalog's last refresh.</summary>
/// <param name="AcceptanceRatePercent">Accepted submissions as a percentage of all submissions: LeetCode's success rate.</param>
/// <param name="TotalAccepted">How many submissions were accepted.</param>
/// <param name="TotalSubmissions">How many submissions were made.</param>
/// <param name="Likes">How many users liked the question.</param>
/// <param name="Dislikes">How many users disliked the question.</param>
public sealed record LeetCodeStatistics(
    double AcceptanceRatePercent,
    long TotalAccepted,
    long TotalSubmissions,
    int Likes,
    int Dislikes);
