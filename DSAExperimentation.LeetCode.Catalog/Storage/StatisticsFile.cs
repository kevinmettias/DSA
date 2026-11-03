namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Snapshot/Statistics.json: one date and every enrolled question's figures as of it, so the whole
// file is one snapshot and its date says how old every number in it is. Questions are written in
// ordinal slug order, so a refresh's diff shows only the figures that moved.
/// <summary>Every question's statistics as of one refresh.</summary>
/// <param name="AsOf">The day the figures were read from LeetCode.</param>
/// <param name="Questions">Each question's figures, by title slug.</param>
internal sealed record StatisticsFile(DateOnly AsOf, IReadOnlyDictionary<string, LeetCodeStatistics> Questions)
{
    private const string FileName = "Statistics.json";

    private static readonly StatisticsFile Empty = new(DateOnly.MinValue, new Dictionary<string, LeetCodeStatistics>());

    private static string FilePath => Path.Combine(CatalogJson.SnapshotDirectory, FileName);

    public static StatisticsFile Read() => CatalogJson.TryRead<StatisticsFile>(FilePath, out var file) ? file : Empty;

    public bool TryWrite()
    {
        var ordered = new SortedDictionary<string, LeetCodeStatistics>(Questions.ToDictionary(), StringComparer.Ordinal);
        return CatalogJson.TryWrite(FilePath, this with { Questions = ordered });
    }
}
