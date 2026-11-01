namespace DSAExperimentation.DataStructures.Graph.Grids;

// The shared, read-only context a GridNode carries a reference to - the same
// pattern every other topology relies on (a node knows how to find its own
// neighbors), just for adjacency computed from geometry instead of stored as an
// explicit list. GetChildren stays a pure function of the node because the node
// carries this reference; GridTopology itself stays a stateless witness like every
// other topology in this library.
/// <summary>
/// A rectangular board of open and blocked cells, shared by every <see cref="GridNode"/> on it.
/// </summary>
/// <param name="passable">Which cells are open, indexed [row, col]; its dimensions are the board's.</param>
public sealed class Grid(bool[,] passable)
{
    /// <summary>The board's extent: its row and column counts.</summary>
    public GridSize Size { get; } = new(passable.GetLength(0), passable.GetLength(1));

    /// <summary>How many rows the board has.</summary>
    public int Rows => Size.Rows;

    /// <summary>How many columns the board has.</summary>
    public int Cols => Size.Cols;

    // Every cell open: a board whose only walls are its edges.
    public Grid(int rows, int cols)
        : this(AllOpen(rows, cols))
    {
    }

    /// <summary>Reports whether a cell is on the board and open.</summary>
    /// <param name="row">The cell's row, which may lie off the board.</param>
    /// <param name="col">The cell's column, which may lie off the board.</param>
    /// <returns>True when the cell is on the board and open; false when it is off the board or blocked.</returns>
    public bool IsPassable(int row, int col) => Size.HasCell(row, col) && passable[row, col];

    private static bool[,] AllOpen(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                passable[row, col] = true;
            }
        }

        return passable;
    }
}
