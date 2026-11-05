using DSAExperimentation.LeetCode.Solutions.CountLatticePointsInsideACircle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountLatticePointsInsideACircle;

// Harness only. Both strategies are CountLatticePointsInsideACircleSolution's, and
// pinning them to the same cases is what finally puts the full-grid baseline -
// previously a private helper in the benchmark, asserted by nothing - under test
// alongside the Set-backed union it is measured against.
public sealed partial class CountLatticePointsInsideACircleSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // Two unit circles sharing exactly (0,0) and (1,0): the union is 8,
            // not the naive 10, only if the shared points are de-duplicated.
            { [[0, 0, 1], [1, 0, 1]], 8 },

            // Far apart, so nothing is shared and the union is the plain sum.
            { [[0, 0, 1], [10, 10, 1]], 10 },

            // Fully nested: the inner circle contributes nothing new at all.
            { [[0, 0, 2], [0, 0, 1]], 13 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountLatticePointsByFullGridScan_Cases_ReturnsDistinctCoveredLatticePointCount(
        int[][] circles, int expected) =>
        Assert.Equal(expected, CountLatticePointsInsideACircleSolution.CountLatticePointsByFullGridScan(circles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountLatticePointsByPerCircleSetUnion_Cases_ReturnsDistinctCoveredLatticePointCount(
        int[][] circles, int expected) =>
        Assert.Equal(
            expected,
            CountLatticePointsInsideACircleSolution.CountLatticePointsByPerCircleSetUnion(circles));
}
