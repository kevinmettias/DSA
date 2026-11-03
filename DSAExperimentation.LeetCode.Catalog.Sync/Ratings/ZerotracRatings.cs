using System.Net.Http.Json;
using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Ratings;

// zerotrac/leetcode_problem_rating (MIT) publishes one JSON array of every rated contest question,
// refreshed after each contest. The catalog keeps the entries for the questions it enrols.
internal static class ZerotracRatings
{
    public const string RatingsUrl = "https://raw.githubusercontent.com/zerotrac/leetcode_problem_rating/main/data.json";
    public const string ProjectUrl = "https://github.com/zerotrac/leetcode_problem_rating";

    public static async Task<IReadOnlyList<ZerotracRatingDto>> FetchAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var ratings = await client.GetFromJsonAsync<List<ZerotracRatingDto>>(RatingsUrl, cancellationToken);
        return ratings ?? throw new InvalidOperationException($"{RatingsUrl} returned no ratings.");
    }

    public static ContestRatingsFile ForEnrolled(IReadOnlyList<ZerotracRatingDto> ratings, IReadOnlySet<string> enrolledSlugs)
        => new(
            ProjectUrl,
            ratings
                .Where(rating => enrolledSlugs.Contains(rating.TitleSlug))
                .DistinctBy(rating => rating.TitleSlug)
                .ToDictionary(
                    rating => rating.TitleSlug,
                    rating => new LeetCodeContestRating(rating.Rating, rating.ContestSlug, rating.ProblemIndex)));
}
