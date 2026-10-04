namespace DSAExperimentation.DataStructures.Graph.Grids;

// A rectangular board's extent, and the one bounds rule every grid search asks: is (row, col) on it?
// Grid answers "on the board and open" from a fixed passability map; this answers only the first
// half, for searches whose notion of "open" is a property of the problem's own cells (land, water,
// a height no lower than the last) rather than a map known up front. A value type, so passing or
// capturing one costs nothing.
internal readonly record struct GridSize(int Rows, int Cols)
{
    // Rows from the outer array, columns from the first row: LeetCode's grids are rectangular, so
    // every row has the first row's length. An empty outer array has no first row to read.
    public static GridSize Of<TCell>(TCell[][] cells) => new(cells.Length, cells[0].Length);

    // Signed comparisons on purpose: an index one step past either edge, in either direction, is
    // off the board, which is what lets a neighbour scan probe past the border unguarded.
    public bool HasCell(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
}
