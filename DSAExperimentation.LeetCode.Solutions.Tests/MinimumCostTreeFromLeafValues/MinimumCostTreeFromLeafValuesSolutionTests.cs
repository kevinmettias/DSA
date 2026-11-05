using DSAExperimentation.LeetCode.Solutions.MinimumCostTreeFromLeafValues;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostTreeFromLeafValues;

// Harness only: both strategies live in MinimumCostTreeFromLeafValuesSolution and
// are asserted against the same cases - LeetCode's two published cases, a run of
// equal leaves that must be combined pairwise, a single leaf with nothing to merge,
// and strictly increasing / decreasing / peaked arrangements that exercise the
// sweep's two halves (merge-on-arrival vs. drain-what-is-left) separately.
// The exponential baseline is asserted here for the first time; the lengths stay
// small because it really is exponential.
public sealed partial class MinimumCostTreeFromLeafValuesSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1, 1], 2 },
            { [7], 0 },
            { [4, 3, 2, 1], 20 },
            { [2, 4, 3], 20 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumCostTreeFromLeafValuesByUnmemoizedRecursion_Cases_ReturnsMinimumNodeSum(
        int[] arr,
        int expected) =>
        Assert.Equal(expected, MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByUnmemoizedRecursion(arr));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumCostTreeFromLeafValuesByMonotonicStack_Cases_ReturnsMinimumNodeSum(
        int[] arr,
        int expected) =>
        Assert.Equal(expected, MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByMonotonicStack(arr));
}
