using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// BreadthFirstDistances fixed to the grid's node, topology and children, read for one target - see
// DistanceMapReduceAlgebra for why this particular runtime-parameterized query doesn't need a bespoke
// traversal, unlike LowestCommonAncestor.
/// <summary>Counts the fewest orthogonal moves between two cells of a grid, routing around its walls.</summary>
public static class GridShortestPath
{
    /// <summary>Counts the fewest orthogonal moves, each onto a passable cell, from one cell to another.</summary>
    /// <param name="start">The cell the path begins on.</param>
    /// <param name="target">The cell the path ends on, in the same grid as <paramref name="start"/>.</param>
    /// <returns>
    /// The number of moves, 0 when the two cells are the same, or <c>null</c> when no path reaches <paramref
    /// name="target"/>.
    /// </returns>
    public static int? Distance(GridNode start, GridNode target)
    {
        var distances = BreadthFirstDistances.From<GridNode, GridTopology, GridChildren>(start);

        return distances.TryGetValue(target, out var distance) ? distance : null;
    }
}
