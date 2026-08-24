using DSAExperimentation.Graph.Algorithms.ShortestPaths;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

// Straight-line (Manhattan) distance on a unit-weight 4-directional grid: never
// overestimates the true remaining distance, even once a wall forces a detour, since
// removing cells can only make the true shortest path longer than the unobstructed
// straight line - which is exactly what "admissible" requires.
internal readonly struct ManhattanHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : ManhattanDistance(node, target);

    private static int ManhattanDistance(WeightedGridNode node, WeightedGridNode target)
        => Math.Abs(node.Row - target.Row) + Math.Abs(node.Col - target.Col);
}
