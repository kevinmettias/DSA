using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MinimumTimeVisitingAllPoints;

// LeetCode 1266. Minimum Time Visiting All Points: the time to move between two
// points is exactly their Chebyshev (king-move) distance, since a diagonal step
// costs the same single unit as an axis-aligned one, so the answer is that
// distance summed over consecutive points.
//
// The two strategies differ only in where the distance comes from: open-coded
// max(|dx|, |dy|), or Algorithms.ShortestPaths' ChebyshevHeuristic - the same witness
// ShortestPath.AStar composes for an 8-directional grid, used here purely for its
// distance computation rather than an actual search.
internal static class MinimumTimeVisitingAllPointsSolution
{
    public static int MinTimeByInlineChebyshev(int[][] points)
    {
        var total = 0;

        for (var i = 1; i < points.Length; i++)
        {
            var deltaX = Math.Abs(points[i][0] - points[i - 1][0]);
            var deltaY = Math.Abs(points[i][1] - points[i - 1][1]);
            total += Math.Max(deltaX, deltaY);
        }

        return total;
    }

    public static int MinTimeByPathHeuristic(int[][] points)
    {
        var total = 0;

        for (var i = 1; i < points.Length; i++)
        {
            var from = new WeightedGridNode(points[i - 1][0], points[i - 1][1]);
            var to = new WeightedGridNode(points[i][0], points[i][1]);

            total += ChebyshevHeuristic.Estimate(from, to);
        }

        return total;
    }
}
