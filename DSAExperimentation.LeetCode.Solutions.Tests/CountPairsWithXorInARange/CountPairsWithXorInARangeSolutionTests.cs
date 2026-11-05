using DSAExperimentation.LeetCode.Solutions.CountPairsWithXorInARange;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountPairsWithXorInARange;

// Harness only. Both counting strategies are CountPairsWithXorInARangeSolution's;
// this file pins them to LeetCode example 2's array over the narrower [6, 10] band
// the original coverage used in place of [5, 14], the wide-open band, a single value
// with no pair at all, a duplicate-heavy input whose only in-range XOR is zero, and
// the low == high == 0 band that makes the trie arm subtract a CountXorBelow(value, 0)
// that must come to zero: no XOR is below 0, so the walk passes every level without
// adding a count. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountPairsWithXorInARangeSolutionTests
{
    public static TheoryData<int[], int, int, int> Cases =>
        new()
        {
            { [9, 8, 4, 2, 1], 6, 10, 4 },
            { [1, 2, 3, 4], 0, 100, 6 },
            { [5], 0, 100, 0 },
            { [3, 3, 3], 0, 0, 3 },
            { [1, 4, 2, 7], 0, 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByPairwiseScan_Cases_ReturnsPairCountInRange(
        int[] nums, int low, int high, int expected)
    {
        var actual = CountPairsWithXorInARangeSolution.CountPairsByPairwiseScan(nums, low, high);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByBitTrieRangeCount_Cases_ReturnsPairCountInRange(
        int[] nums, int low, int high, int expected)
    {
        var actual = CountPairsWithXorInARangeSolution.CountPairsByBitTrieRangeCount(nums, low, high);
        Assert.Equal(expected, actual);
    }
}
