using DSAExperimentation.LeetCode.ReversePairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReversePairs;

// Harness only. Both strategies are ReversePairsSolution's - this file just pins
// them to LeetCode's published examples, including the pairwise-scan baseline that
// was never asserted anywhere before this migration.
public sealed partial class ReversePairsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 2, 3, 1], 2 },
            { [2, 4, 3, 5, 1], 3 },
            { [1, 2, 3, 4], 0 },
            // 2 * int.MinValue overflows a 32-bit accumulator - a wrapped-to-zero
            // threshold would silently miss this pair, proving the algorithm's long
            // threshold is load-bearing, not incidental.
            { [int.MinValue, int.MinValue], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPairwiseScan_LeetCodeExamples_ReturnsReversePairCount(int[] nums, int expected) =>
        Assert.Equal(expected, ReversePairsSolution.CountByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickTreeSweep_LeetCodeExamples_ReturnsReversePairCount(int[] nums, int expected) =>
        Assert.Equal(expected, ReversePairsSolution.CountByFenwickTreeSweep(nums));
}
