using DSAExperimentation.LeetCode.CountLatticePointsInsideACircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountLatticePointsInsideACircle;

// Harness only. Both strategies are CountLatticePointsInsideACircleSolution's, and
// pinning them to the same examples is what finally puts the full-grid baseline -
// previously a private helper in the benchmark, asserted by nothing - under test
// alongside the Set-backed union it is measured against.
public sealed class CountLatticePointsInsideACircleTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: the plus shape around (2,2).
            { [[2, 2, 1]], 5 },

            // LeetCode example 2: 13 points inside the radius-2 circle plus the
            // three the radius-1 circle adds, (3,3) and (2,4) already counted.
            { [[2, 2, 2], [3, 4, 1]], 16 },

            // Two unit circles sharing exactly (0,0) and (1,0): the union is 8,
            // not the naive 10, only if the shared points are de-duplicated.
            { [[0, 0, 1], [1, 0, 1]], 8 },

            // Far apart, so nothing is shared and the union is the plain sum.
            { [[0, 0, 1], [10, 10, 1]], 10 },

            // Fully nested: the inner circle contributes nothing new at all.
            { [[0, 0, 2], [0, 0, 1]], 13 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountLatticePointsByFullGridScan_LeetCodeExamples_ReturnsDistinctCoveredLatticePointCount(
        int[][] circles, int expected) =>
        Assert.Equal(expected, CountLatticePointsInsideACircleSolution.CountLatticePointsByFullGridScan(circles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountLatticePointsByPerCircleSetUnion_LeetCodeExamples_ReturnsDistinctCoveredLatticePointCount(
        int[][] circles, int expected) =>
        Assert.Equal(
            expected,
            CountLatticePointsInsideACircleSolution.CountLatticePointsByPerCircleSetUnion(circles));
}
