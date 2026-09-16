using DSAExperimentation.LeetCode.MinimumCostTreeFromLeafValues;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostTreeFromLeafValues;

// Harness only: both strategies live in MinimumCostTreeFromLeafValuesSolution and
// are asserted against the same examples - LeetCode's two published cases, a run of
// equal leaves that must be combined pairwise, a single leaf with nothing to merge,
// and strictly increasing / decreasing / peaked arrangements that exercise the
// sweep's two halves (merge-on-arrival vs. drain-what-is-left) separately.
// The exponential baseline is asserted here for the first time; the lengths stay
// small because it really is exponential.
public sealed partial class MinimumCostTreeFromLeafValuesTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [6, 2, 4], 32 },
            { [4, 11], 44 },
            { [1, 1, 1], 2 },
            { [7], 0 },
            { [1, 2, 3, 4], 20 },
            { [4, 3, 2, 1], 20 },
            { [2, 4, 3], 20 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostTreeFromLeafValuesByUnmemoizedRecursion_LeetCodeExamples_ReturnsMinimumNodeSum(
        int[] arr,
        int expected) =>
        Assert.Equal(expected, MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByUnmemoizedRecursion(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostTreeFromLeafValuesByMonotonicStack_LeetCodeExamples_ReturnsMinimumNodeSum(
        int[] arr,
        int expected) =>
        Assert.Equal(expected, MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByMonotonicStack(arr));
}
