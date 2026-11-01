namespace DSAExperimentation.DataStructures.Graph.Grids;

// A weighted, coordinate-carrying node - distinct from a plain weighted node
// because ShortestPath.AStar needs geometry to compute an admissible heuristic
// against, which arbitrary edge weights don't provide.
/// <summary>
/// A weighted-graph node that carries its grid coordinates, for searches such as A* whose heuristic needs them.
/// </summary>
/// <param name="row">The node's row.</param>
/// <param name="col">The node's column.</param>
public sealed class WeightedGridNode(int row, int col)
{
    /// <summary>The node's row on the grid.</summary>
    public int Row { get; } = row;

    /// <summary>The node's column on the grid.</summary>
    public int Col { get; } = col;

    /// <summary>The node's outgoing edges, each the step's weight and the node it leads to.</summary>
    public List<(int Weight, WeightedGridNode Target)> Edges { get; } = [];

    public override string ToString() => $"({Row},{Col})";
}
