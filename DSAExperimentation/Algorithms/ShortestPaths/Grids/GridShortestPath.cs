using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// BreadthFirstDistances.To fixed to the grid's node, topology and children: one target's distance,
// from a breadth-first search that stops at the target rather than mapping the whole board.
/// <summary>Counts the fewest orthogonal moves between two cells of a grid, routing around its walls.</summary>
public static class GridShortestPath
{
    /// <summary>Counts the fewest orthogonal moves, each onto a passable cell, from one cell to another.</summary>
    /// <typeparam name="TNodes">Which node the search is handed for each cell it reaches.</typeparam>
    /// <param name="start">The cell the path begins on.</param>
    /// <param name="target">The cell the path ends on, in the same grid as <paramref name="start"/>.</param>
    /// <returns>
    /// The number of moves, 0 when the two cells are the same, or <c>null</c> when no path reaches <paramref
    /// name="target"/>.
    /// </returns>
    public static int? Distance<TNodes>(GridNode start, GridNode target)
        where TNodes : struct, IGridNodeSource
        => BreadthFirstDistances.To<GridNode, GridTopology<TNodes>, GridChildren<TNodes>>(start, target);
}
