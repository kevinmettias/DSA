namespace DSAExperimentation.DataStructures.Graph.Grids;

// The shared, read-only context a GridNode carries a reference to - the same
// pattern every other topology relies on (a node knows how to find its own
// neighbors), just for adjacency computed from geometry instead of stored as an
// explicit list. GetChildren stays a pure function of the node because the node
// carries this reference; GridTopology itself stays a stateless witness like every
// other topology in this library.
internal sealed class Grid(bool[,] passable)
{
    public GridSize Size { get; } = new(passable.GetLength(0), passable.GetLength(1));

    public int Rows => Size.Rows;
    public int Cols => Size.Cols;

    // Every cell open: a board whose only walls are its edges.
    public Grid(int rows, int cols)
        : this(AllOpen(rows, cols))
    {
    }

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
