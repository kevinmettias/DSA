using DSAExperimentation.LeetCode.RangeFrequencyQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeFrequencyQueries;

// Harness only. Both strategies are RangeFrequencyQueriesSolution's - this file
// just pins them to LeetCode's published example queries, plus a value that never
// occurs, a single-index window, and a window that excludes some occurrences.
public sealed partial class RangeFrequencyQueriesTests
{
    public static TheoryData<RangeFrequencyCase> Examples =>
        new()
        {
            { new RangeFrequencyCase(Arr: [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], Left: 1, Right: 2, Value: 4, Expected: 1) },
            { new RangeFrequencyCase(Arr: [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], Left: 0, Right: 11, Value: 33, Expected: 2) },
            { new RangeFrequencyCase(Arr: [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], Left: 0, Right: 11, Value: 22, Expected: 2) },
            { new RangeFrequencyCase(Arr: [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], Left: 0, Right: 3, Value: 33, Expected: 1) },
            { new RangeFrequencyCase(Arr: [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], Left: 5, Right: 5, Value: 2, Expected: 1) },
            { new RangeFrequencyCase(Arr: [1, 2, 3], Left: 0, Right: 2, Value: 99, Expected: 0) },
            { new RangeFrequencyCase(Arr: [7], Left: 0, Right: 0, Value: 7, Expected: 1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueryByBinarySearchIndex_LeetCodeExamples_ReturnsFrequencyOfValueInSubarray(
        RangeFrequencyCase example)
    {
        var actual = RangeFrequencyQueriesSolution.QueryByBinarySearchIndex(
            example.Arr, example.Left, example.Right, example.Value);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueryByBruteForceRescan_LeetCodeExamples_ReturnsFrequencyOfValueInSubarray(
        RangeFrequencyCase example)
    {
        var actual = RangeFrequencyQueriesSolution.QueryByBruteForceRescan(
            example.Arr, example.Left, example.Right, example.Value);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the array, the inclusive window as its left and right
    // indices, the value whose occurrences are counted, and how many land inside the
    // window. Left, Right and Value are all bare ints, so each is named at every
    // construction site and a row reads as the case it is rather than as three
    // positions a caller has to keep in order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct RangeFrequencyCase(int[] Arr, int Left, int Right, int Value, int Expected);
}
