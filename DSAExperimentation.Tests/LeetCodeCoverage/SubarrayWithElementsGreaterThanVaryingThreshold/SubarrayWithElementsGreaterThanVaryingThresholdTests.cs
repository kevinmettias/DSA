using DSAExperimentation.LeetCode.SubarrayWithElementsGreaterThanVaryingThreshold;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarrayWithElementsGreaterThanVaryingThreshold;

// Harness only. Both strategies are
// SubarrayWithElementsGreaterThanVaryingThresholdSolution's - this file pins them to
// LeetCode's published examples plus the degenerate cases those never reach.
//
// LC 2334 accepts *any* qualifying subarray size, so each example states every size
// that qualifies rather than one blessed number, and the two strategies genuinely
// exercise that latitude: on LeetCode's own [6,5,6,5,8] example the window scan
// returns 2 (the first qualifying window in start order) while the union-find sweep
// returns 1 (the first qualifying window in value order). Most examples here are
// pinned to a single admissible size, so the freedom is not load-bearing anywhere the
// answer is actually determined.
public sealed partial class SubarrayWithElementsGreaterThanVaryingThresholdTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 3, 4, 3, 1], 6, [3] },
            { [6, 5, 6, 5, 8], 7, [1, 2, 3, 4, 5] },
            { [1, 1, 1, 1], 1000, [-1] },
            { [9], 8, [1] },
            { [5, 5], 6, [2] },
            { [2, 2, 2], 5, [3] },
            { [3, 1, 3], 5, [-1] },
            { [10, 1, 10], 9, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidSubarraySizeByWindowMinimumScan_LeetCodeExamples_ReturnsAQualifyingSize(
        int[] nums, int threshold, int[] qualifyingSizes)
    {
        var size = SubarrayWithElementsGreaterThanVaryingThresholdSolution.ValidSubarraySizeByWindowMinimumScan(
            nums,
            threshold);

        Assert.Contains(size, qualifyingSizes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidSubarraySizeByUnionFindOrder_LeetCodeExamples_ReturnsAQualifyingSize(
        int[] nums, int threshold, int[] qualifyingSizes)
    {
        var size = SubarrayWithElementsGreaterThanVaryingThresholdSolution.ValidSubarraySizeByUnionFindOrder(
            nums,
            threshold);

        Assert.Contains(size, qualifyingSizes);
    }
}
