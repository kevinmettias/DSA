using DSAExperimentation.LeetCode.Solutions.ReversePairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReversePairs;

// Harness only. Both strategies are ReversePairsSolution's, including the
// pairwise-scan baseline that was never asserted anywhere before this migration -
// this file just pins them to an ascending array with no reverse pair and a pair
// whose doubled value overflows 32 bits. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ReversePairsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 3, 4], 0 },
            // 2 * int.MinValue overflows a 32-bit accumulator - a wrapped-to-zero
            // threshold would silently miss this pair, proving the algorithm's long
            // threshold is load-bearing, not incidental.
            { [int.MinValue, int.MinValue], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByPairwiseScan_Cases_ReturnsReversePairCount(int[] nums, int expected) =>
        Assert.Equal(expected, ReversePairsSolution.CountByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByFenwickTreeSweep_Cases_ReturnsReversePairCount(int[] nums, int expected) =>
        Assert.Equal(expected, ReversePairsSolution.CountByFenwickTreeSweep(nums));
}
