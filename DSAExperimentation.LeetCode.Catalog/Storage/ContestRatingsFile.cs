namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Snapshot/ContestRatings.json: the rated questions only. A question absent from it never appeared
// in a contest, which is a fact about the question, not a gap in the file. Questions are written in
// ordinal slug order, so a refresh's diff shows only the ratings that moved.
/// <summary>The contest ratings of every enrolled question that has one.</summary>
/// <param name="Source">Where the ratings were read from.</param>
/// <param name="Questions">Each rated question's rating, by title slug.</param>
internal sealed record ContestRatingsFile(string Source, IReadOnlyDictionary<string, LeetCodeContestRating> Questions)
{
    private const string FileName = "ContestRatings.json";

    private static readonly ContestRatingsFile Empty = new(string.Empty, new Dictionary<string, LeetCodeContestRating>());

    private static string FilePath => Path.Combine(CatalogJson.SnapshotDirectory, FileName);

    public static ContestRatingsFile Read() => CatalogJson.TryRead<ContestRatingsFile>(FilePath, out var file) ? file : Empty;

    public bool TryWrite()
    {
        var ordered = new SortedDictionary<string, LeetCodeContestRating>(Questions.ToDictionary(), StringComparer.Ordinal);
        return CatalogJson.TryWrite(FilePath, this with { Questions = ordered });
    }
}
