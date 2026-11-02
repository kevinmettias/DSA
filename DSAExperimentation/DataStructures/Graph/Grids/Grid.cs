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

    // The board a filter opens: a flood fill over a problem's own cells - land, water, a cell
    // holding fish - asks its IGridCellFilter about each cell once, here, and walks the fixed map
    // afterwards. The filter is a struct witness, so each rule is its own instantiation and
    // CanEnter a direct call; every cell it is asked about is on the board.
    /// <summary>Builds the board whose open cells are the ones a filter accepts.</summary>
    /// <param name="size">The board's extent.</param>
    /// <param name="filter">The rule a cell must pass to be open, asked once about each cell.</param>
    /// <returns>A board of <paramref name="size"/>, open exactly where <paramref name="filter"/> accepted.</returns>
    public static Grid Where<TFilter>(GridSize size, TFilter filter)
        where TFilter : struct, IGridCellFilter
    {
        var open = new bool[size.Rows, size.Cols];

        for (var row = 0; row < size.Rows; row++)
        {
            for (var col = 0; col < size.Cols; col++)
            {
                open[row, col] = filter.CanEnter(row, col);
            }
        }

        return new Grid(open);
    }

    // Every open cell as a node of this grid, row by row: the roots a whole-board flood fill scans.
    /// <summary>Lists every open cell as a node of this board, row by row and left to right.</summary>
    /// <returns>A node for each open cell, each on this board.</returns>
    public IEnumerable<GridNode> PassableNodes()
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                if (IsPassable(row, col))
                {
                    yield return new GridNode(row, col, this);
                }
            }
        }
    }

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
