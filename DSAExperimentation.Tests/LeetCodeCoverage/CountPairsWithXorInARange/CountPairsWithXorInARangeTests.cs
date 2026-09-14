using DSAExperimentation.LeetCode.CountPairsWithXorInARange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPairsWithXorInARange;

// Harness only. Both counting strategies are CountPairsWithXorInARangeSolution's;
// this file pins them to LeetCode's two published examples - including the
// [5, 14] band the original coverage replaced with a narrower one of its own - plus
// the wide-open band, a single value with no pair at all, a duplicate-heavy input
// whose only in-range XOR is zero, and the low == high == 0 band that makes the
// trie arm subtract a CountLessThan(0) it must answer without descending.
public sealed class CountPairsWithXorInARangeTests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            { [1, 4, 2, 7], 2, 6, 6 },
            { [9, 8, 4, 2, 1], 5, 14, 8 },
            { [9, 8, 4, 2, 1], 6, 10, 4 },
            { [1, 2, 3, 4], 0, 100, 6 },
            { [5], 0, 100, 0 },
            { [3, 3, 3], 0, 0, 3 },
            { [1, 4, 2, 7], 0, 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByPairwiseScan_LeetCodeExamples_ReturnsPairCountInRange(
        int[] nums, int low, int high, int expected) =>
        Assert.Equal(expected, CountPairsWithXorInARangeSolution.CountPairsByPairwiseScan(nums, low, high));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBitTrieRangeCount_LeetCodeExamples_ReturnsPairCountInRange(
        int[] nums, int low, int high, int expected) =>
        Assert.Equal(expected, CountPairsWithXorInARangeSolution.CountPairsByBitTrieRangeCount(nums, low, high));
}
