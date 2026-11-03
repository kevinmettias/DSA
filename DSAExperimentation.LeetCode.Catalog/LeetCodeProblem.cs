using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog;

// The three things the catalog knows about a problem, from three sources refreshed on their own
// schedules, put back together for a reader that wants the whole picture.
/// <summary>Everything the catalog knows about one problem: the question, its statistics and its contest rating.</summary>
/// <param name="Question">The question as LeetCode publishes it.</param>
/// <param name="Statistics">How LeetCode's users have fared on it, including its acceptance rate.</param>
/// <param name="ContestRating">Its rating from contest results, or null if it never appeared in a contest.</param>
public sealed record LeetCodeProblem(
    LeetCodeQuestion Question,
    LeetCodeStatistics Statistics,
    LeetCodeContestRating? ContestRating);
