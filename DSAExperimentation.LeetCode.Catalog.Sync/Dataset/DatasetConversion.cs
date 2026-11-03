namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// What converting one question's dataset row produced: the cases kept, and how many of the rest
// were dropped for each reason.
internal sealed record DatasetConversion(
    IReadOnlyList<LeetCodeCase> Cases,
    IReadOnlyDictionary<DatasetDropReason, int> Dropped)
{
    // A question the dataset does not cover, or covers in a shape it cannot import.
    public static DatasetConversion None { get; } = new([], new Dictionary<DatasetDropReason, int>());
}
