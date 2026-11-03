namespace DSAExperimentation.DataStructures.Graph.Grids;

/// <summary>A new node each time a cell is asked for, with no table: for a walk that touches few of a board's cells.</summary>
public readonly struct NodePerStep : IGridNodeSource
{
    /// <inheritdoc/>
    public static GridNode NodeAt(Grid grid, int row, int col) => new(row, col, grid);
}
