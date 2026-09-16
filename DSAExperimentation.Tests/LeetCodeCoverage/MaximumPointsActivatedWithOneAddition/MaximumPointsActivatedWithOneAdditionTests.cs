using DSAExperimentation.LeetCode.MaximumPointsActivatedWithOneAddition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumPointsActivatedWithOneAddition;

// Harness only. The bipartite x/y reduction lives in
// MaximumPointsActivatedWithOneAdditionSolution - this file just pins both
// strategies to LeetCode's published examples.
public sealed partial class MaximumPointsActivatedWithOneAdditionTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { new[] { new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 2 } }, 4 },
            { new[] { new[] { 2, 2 }, new[] { 1, 1 }, new[] { 3, 3 } }, 3 },
            { new[] { new[] { 2, 3 }, new[] { 2, 2 }, new[] { 1, 1 }, new[] { 4, 5 } }, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxActivatedByBruteForceUnionFind_LeetCodeExamples_ReturnsMaximumActivatedCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaximumPointsActivatedWithOneAdditionSolution.MaxActivatedByBruteForceUnionFind(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxActivatedByKeyedDisjointSet_LeetCodeExamples_ReturnsMaximumActivatedCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaximumPointsActivatedWithOneAdditionSolution.MaxActivatedByKeyedDisjointSet(points));
}
