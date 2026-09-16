using DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostPathWithAlternatingDirectionsIII;

// Harness only. The alternating-parity state search is
// MinimumCostPathWithAlternatingDirectionsIIISolution's; this file just pins both
// strategies to LeetCode's published examples.
public sealed class MinimumCostPathWithAlternatingDirectionsIIITests
{
    public static TheoryData<int, int, int[][], long> Examples =>
        new()
        {
            { 2, 2, new[] { new[] { 5, 3 }, new[] { 1, 4 } }, 8 },
            { 2, 2, new[] { new[] { 0, 7 }, new[] { 3, 2 } }, 7 },
            { 2, 3, new[] { new[] { 8, 0, 9 }, new[] { 7, 4, 1 } }, 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByBclDijkstra_LeetCodeExamples_ReturnsMinimumTotalCost(
        int m, int n, int[][] penalty, long expected)
    {
        var actual = MinimumCostPathWithAlternatingDirectionsIIISolution.MinCostByBclDijkstra(m, n, penalty);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByStateDijkstra_LeetCodeExamples_ReturnsMinimumTotalCost(
        int m, int n, int[][] penalty, long expected)
    {
        var actual = MinimumCostPathWithAlternatingDirectionsIIISolution.MinCostByStateDijkstra(m, n, penalty);

        Assert.Equal(expected, actual);
    }
}
