using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// Straight-line (Manhattan) distance on a unit-weight 4-directional grid:
// consistent, not merely admissible - moving to an adjacent cell changes the
// Manhattan distance to target by at most 1, which equals that edge's own weight,
// so h(u) <= cost(u,v) + h(v) holds on every edge (the triangle inequality
// IPathHeuristic requires). It's also never an overestimate: walls only remove
// cells, which can make the true shortest path longer than the unobstructed
// straight line, never shorter.
/// <summary>
/// Estimates a cell's remaining distance as its Manhattan distance to the target, consistent when every move
/// is one orthogonal step costing 1.
/// </summary>
public readonly struct ManhattanHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : ManhattanDistance(node, target);

    private static int ManhattanDistance(WeightedGridNode node, WeightedGridNode target)
        => Math.Abs(node.Row - target.Row) + Math.Abs(node.Col - target.Col);
}
