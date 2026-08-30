using DSAExperimentation.Algorithms.ShortestPaths;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Consistent on an open, unit-weight 8-directional grid (see the Tests fixture of
// the same name for the full proof) - king-move distance, exact whenever the grid
// has no obstacles between node and target.
internal readonly struct ChebyshevHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : Math.Max(Math.Abs(node.Row - target.Row), Math.Abs(node.Col - target.Col));
}
