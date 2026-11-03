using DSAExperimentation.LeetCode.Catalog.Storage;
using DSAExperimentation.LeetCode.Catalog.Sync.Ratings;

namespace DSAExperimentation.LeetCode.Catalog.Sync;

// `ratings`: rewrites ContestRatings.json from zerotrac's current data, for the enrolled questions.
internal static class RatingsCommand
{
    public const string Verb = "ratings";

    public static async Task<int> RunAsync(TextWriter output, CancellationToken cancellationToken)
    {
        var enrolled = QuestionFiles.Slugs().ToHashSet(StringComparer.Ordinal);
        var ratings = await ZerotracRatings.FetchAsync(cancellationToken);
        var file = ZerotracRatings.ForEnrolled(ratings, enrolled);
        var isChanged = file.TryWrite();

        output.WriteLine($"{file.Questions.Count} of {enrolled.Count} questions have a contest rating; changed: {isChanged}.");
        return 0;
    }
}
