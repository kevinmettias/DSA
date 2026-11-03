namespace DSAExperimentation.DataStructures.Graph.Grids;

/// <summary>The board's one node for each cell, made once: for a walk that reaches cells from several sides.</summary>
public readonly struct NodePerCell : IGridNodeSource
{
    /// <inheritdoc/>
    public static GridNode NodeAt(Grid grid, int row, int col) => grid.NodeAt(row, col);
}
