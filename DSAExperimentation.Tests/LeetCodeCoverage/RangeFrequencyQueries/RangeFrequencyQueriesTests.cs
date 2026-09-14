using DSAExperimentation.LeetCode.RangeFrequencyQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeFrequencyQueries;

// Harness only. Both strategies are RangeFrequencyQueriesSolution's - this file
// just pins them to LeetCode's published example queries, plus a value that never
// occurs, a single-index window, and a window that excludes some occurrences.
public sealed class RangeFrequencyQueriesTests
{
    public static TheoryData<int[], int, int, int, int> Examples =>
        new()
        {
            { [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], 1, 2, 4, 1 },
            { [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], 0, 11, 33, 2 },
            { [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], 0, 11, 22, 2 },
            { [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], 0, 3, 33, 1 },
            { [12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56], 5, 5, 2, 1 },
            { [1, 2, 3], 0, 2, 99, 0 },
            { [7], 0, 0, 7, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueryByBinarySearchIndex_LeetCodeExamples_ReturnsFrequencyOfValueInSubarray(
        int[] arr, int left, int right, int value, int expected) =>
        Assert.Equal(expected, RangeFrequencyQueriesSolution.QueryByBinarySearchIndex(arr, left, right, value));

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueryByBruteForceRescan_LeetCodeExamples_ReturnsFrequencyOfValueInSubarray(
        int[] arr, int left, int right, int value, int expected) =>
        Assert.Equal(expected, RangeFrequencyQueriesSolution.QueryByBruteForceRescan(arr, left, right, value));
}
