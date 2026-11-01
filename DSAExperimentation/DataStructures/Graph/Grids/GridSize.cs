namespace DSAExperimentation.DataStructures.Graph.Grids;

// A rectangular board's extent, and the one bounds rule every grid search asks: is (row, col) on it?
// Grid answers "on the board and open" from a fixed passability map; this answers only the first
// half, for searches whose notion of "open" is a property of the problem's own cells (land, water,
// a height no lower than the last) rather than a map known up front. A value type, so passing or
// capturing one costs nothing.
/// <summary>A rectangular board's extent, and the test of whether a cell lies on it.</summary>
/// <param name="Rows">How many rows the board has.</param>
/// <param name="Cols">How many columns the board has.</param>
public readonly record struct GridSize(int Rows, int Cols)
{
    // Rows from the outer array, columns from the first row: LeetCode's grids are rectangular, so
    // every row has the first row's length. An empty outer array has no first row to read.
    /// <summary>Reads the extent of a rectangular jagged grid.</summary>
    /// <param name="cells">The grid's rows, each as long as the first; there must be at least one.</param>
    /// <returns>The grid's row count and its first row's length.</returns>
    public static GridSize Of<TCell>(TCell[][] cells) => new(cells.Length, cells[0].Length);

    // Signed comparisons on purpose: an index one step past either edge, in either direction, is
    // off the board, which is what lets a neighbour scan probe past the border unguarded.
    /// <summary>Reports whether a cell lies on the board.</summary>
    /// <param name="row">The cell's row, which may be negative or past the last row.</param>
    /// <param name="col">The cell's column, which may be negative or past the last column.</param>
    /// <returns>True when both the row and the column fall within the board; otherwise false.</returns>
    public bool HasCell(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
}
