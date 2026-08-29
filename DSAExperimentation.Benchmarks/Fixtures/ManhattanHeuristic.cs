using DSAExperimentation.Algorithms.ShortestPaths;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Consistent on an open, unit-weight 4-directional grid (see the Tests fixture of
// the same name for the full proof) - deliberately open (no walls) here, so the
// heuristic is exact, not merely admissible, and A* explores close to the
// straight-line corridor instead of Dijkstra's full expanding diamond.
internal readonly struct ManhattanHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : Math.Abs(node.Row - target.Row) + Math.Abs(node.Col - target.Col);
}
