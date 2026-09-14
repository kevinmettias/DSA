using DSAExperimentation.LeetCode.NumberOfPairsSatisfyingInequality;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPairsSatisfyingInequality;

// Harness only: both algorithms live in NumberOfPairsSatisfyingInequalitySolution.
// One test method per strategy over one shared set of examples, so a failure names
// the strategy that broke rather than reporting a disagreement between two
// anonymous arms.
public sealed class NumberOfPairsSatisfyingInequalityTests
{
    public static TheoryData<int[], int[], int, long> Examples =>
        new()
        {
            // LeetCode's own two examples.
            { [3, 2, 5], [2, 2, 1], 1, 3L },
            { [3, -1], [-2, 2], -1, 0L },

            // Every difference equal and diff = 0: the condition holds for every
            // pair, so the answer is n choose 2.
            { [1, 1, 1], [1, 1, 1], 0, 3L },

            // A single element has no pair to make.
            { [1], [1], 0, 0L },

            // A negative diff that still admits exactly one pair.
            { [5, 1, 3], [1, 2, 3], -1, 1L },

            // A diff large enough to swamp every difference: all six pairs count.
            { [1, 2, 3, 4], [0, 0, 0, 0], 10, 6L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByPairwiseScan_LeetCodeExamples_ReturnsSatisfyingPairCount(
        int[] nums1, int[] nums2, int diff, long expected) =>
        Assert.Equal(
            expected,
            NumberOfPairsSatisfyingInequalitySolution.CountPairsByPairwiseScan(nums1, nums2, diff));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByFenwickTreeSweep_LeetCodeExamples_ReturnsSatisfyingPairCount(
        int[] nums1, int[] nums2, int diff, long expected) =>
        Assert.Equal(
            expected,
            NumberOfPairsSatisfyingInequalitySolution.CountPairsByFenwickTreeSweep(nums1, nums2, diff));
}
