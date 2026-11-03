namespace DSAExperimentation.DataStructures.Graph.Grids;

// Which node a walk is handed for a cell. A flood fill over a whole board reaches each cell from up to
// four sides, so the board's one node per cell (NodePerCell) is made once instead of once per step. A
// walk that touches a few cells of a large board would pay for a table sized to the whole board, so it
// takes a new node per step (NodePerStep) instead. GridNode is a record, so both give equal nodes for a
// cell: the choice changes what a walk allocates, never which cells it visits or in what order.
/// <summary>Chooses the node a grid walk is handed for a cell.</summary>
public interface IGridNodeSource
{
    /// <summary>A node for a cell of a board, equal to <c>new GridNode(row, col, grid)</c>.</summary>
    /// <param name="grid">The board the cell is on.</param>
    /// <param name="row">The cell's row, on the board.</param>
    /// <param name="col">The cell's column, on the board.</param>
    /// <returns>A node for the cell on <paramref name="grid"/>.</returns>
    static abstract GridNode NodeAt(Grid grid, int row, int col);
}
