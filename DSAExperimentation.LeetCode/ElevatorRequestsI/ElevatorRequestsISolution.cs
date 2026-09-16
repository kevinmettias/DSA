using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.ElevatorRequestsI;

// LeetCode 4020. Elevator Requests I: an elevator starts at floor 0 and moves
// one floor per second, serving requests[] in order; each hop's cost is just
// the distance between two consecutive floors, so the answer is the sum of
// consecutive absolute differences over [0, requests[0], requests[1], ...].
// floorCount bounds what values can appear in requests but never enters the
// arithmetic itself, the same way LeetCode's own signature states it.
//
// One-dimensional distance-between-stops is a degenerate case of the same
// metric MinimumTimeVisitingAllPoints (LC 1266) composes for its own 2-D
// points, so the second strategy reuses Algorithms.ShortestPaths'
// ManhattanHeuristic against WeightedGridNode with every column pinned to 0,
// rather than re-deriving |a-b| by hand a second time.
internal static class ElevatorRequestsISolution
{
    private const int FixedColumn = 0;

    // The textbook answer: track the elevator's current floor and add
    // Math.Abs of every hop. Deliberately BCL-only - the arm the heuristic
    // strategy below has to justify itself against.
    public static int TotalTimeByInlineAbsoluteDifference(int floorCount, int[] requests)
    {
        var floor = 0;
        var total = 0;

        foreach (var request in requests)
        {
            total += Math.Abs(request - floor);
            floor = request;
        }

        return total;
    }

    // Composed: each hop is ManhattanHeuristic.Estimate between two
    // WeightedGridNodes that share a column, so the Manhattan distance
    // collapses to exactly |rowDelta| - the same distance as above, read from
    // Algorithms.ShortestPaths' own witness instead of restated arithmetic.
    public static int TotalTimeByManhattanHeuristic(int floorCount, int[] requests)
    {
        var current = new WeightedGridNode(0, FixedColumn);
        var total = 0;

        foreach (var request in requests)
        {
            var next = new WeightedGridNode(request, FixedColumn);
            total += ManhattanHeuristic.Estimate(current, next);
            current = next;
        }

        return total;
    }
}
