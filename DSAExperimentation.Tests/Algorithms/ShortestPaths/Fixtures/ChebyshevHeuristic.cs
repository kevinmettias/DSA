using DSAExperimentation.Algorithms.ShortestPaths;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

// King-move (Chebyshev) distance on a unit-weight 8-directional grid - consistent,
// not merely admissible, for the same reason ManhattanHeuristic is on its own
// 4-directional grid: moving to any adjacent cell, diagonals included, changes
// max(|dr|,|dc|) to target by at most 1, which equals that move's own unit cost.
// It's also never an overestimate: walls only remove cells, which can make the true
// shortest path longer than the unobstructed king-move distance, never shorter.
internal readonly struct ChebyshevHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : ChebyshevDistance(node, target);

    private static int ChebyshevDistance(WeightedGridNode node, WeightedGridNode target)
        => Math.Max(Math.Abs(node.Row - target.Row), Math.Abs(node.Col - target.Col));
}
