using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeVisitingAllPoints;

// LeetCode 1266. Minimum Time Visiting All Points: the time to move between two
// points is exactly their Chebyshev (king-move) distance, since a diagonal step
// costs the same single unit as an axis-aligned one - this repo's own
// IPathHeuristic<TNode,TWeight> contract, closed over ChebyshevHeuristic (the same
// witness ShortestPath.AStar would use for an 8-directional grid), computes exactly
// that; summing it over consecutive points is the whole answer.
public sealed partial class MinimumTimeVisitingAllPointsTests
{
    [Fact]
    public void MinTimeToVisitAllPoints_ClassicExample_ReturnsSumOfChebyshevDistances()
    {
        int[][] points = [[1, 1], [3, 4], [-1, 0]];

        Assert.Equal(7, MinTimeToVisitAllPoints(points));
    }

    [Fact]
    public void MinTimeToVisitAllPoints_SinglePoint_ReturnsZero()
    {
        int[][] points = [[0, 0]];

        Assert.Equal(0, MinTimeToVisitAllPoints(points));
    }

    private static int MinTimeToVisitAllPoints(int[][] points)
    {
        var total = 0;

        for (var i = 1; i < points.Length; i++)
        {
            var from = new WeightedGridNode($"p{i - 1}", points[i - 1][0], points[i - 1][1]);
            var to = new WeightedGridNode($"p{i}", points[i][0], points[i][1]);

            total += ChebyshevHeuristic.Estimate(from, to);
        }

        return total;
    }
}
