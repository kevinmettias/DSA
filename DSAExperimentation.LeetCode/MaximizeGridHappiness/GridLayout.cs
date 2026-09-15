namespace DSAExperimentation.LeetCode.MaximizeGridHappiness;

// LC 1659's m x n grid reduced to what both strategies' recursion actually reads:
// how many cells there are to fill, and how to read and update the trailing
// occupancy profile that records the last n placements.
//
// The profile is one base-3 digit per cell (0 empty, 1 introvert, 2 extrovert)
// holding a sliding window of the last ColumnCount placements, so the cell above the
// one being filled is the window's oldest digit (mask / OldestDigitScale) and the
// cell to its left is the newest (mask % 3). Placing into a cell shifts a fresh
// digit in and drops the oldest. Since that encoding depends only on the grid's
// shape, it belongs here rather than in either strategy.
//
// Meaningless outside this problem, so it lives beside the solution
// (ARCHITECTURE.md §17.3) rather than in Domain/. It is also the prepared-input
// shape §17.4 calls for: a benchmark builds it once in [GlobalSetup] and hands it to
// the measured method, and since it is not an IEnumerable it can never be confused
// with the (m, n) overload.
internal sealed class GridLayout(int columnCount, int totalCells, int oldestDigitScale)
{
    // One digit per occupancy state: empty, introvert, extrovert.
    private const int MaskBase = 3;

    // The grid's width, which is also the width of the sliding profile window.
    public int ColumnCount { get; } = columnCount;

    // Every cell is visited once, in row-major order, so this is where a walk ends.
    public int TotalCells { get; } = totalCells;

    // The place value of the profile's oldest digit: 3^(ColumnCount - 1).
    public int OldestDigitScale { get; } = oldestDigitScale;

    public static GridLayout Build(int rows, int columns)
    {
        var oldestDigitScale = 1;

        for (var i = 0; i < columns - 1; i++)
        {
            oldestDigitScale *= MaskBase;
        }

        return new GridLayout(columns, rows * columns, oldestDigitScale);
    }

    // The two already-filled cells adjacent to `pos`: the one directly above it and
    // the one directly to its left, reported as 0 (empty) when that cell is off the
    // grid rather than unoccupied.
    public (int Up, int Left) Neighbors(int pos, int mask)
    {
        var row = pos / ColumnCount;
        var col = pos % ColumnCount;

        return (row > 0 ? CellAbove(mask) : 0, col > 0 ? CellToLeft(mask) : 0);
    }

    private int CellAbove(int mask) => mask / OldestDigitScale;

    private int CellToLeft(int mask) => mask % MaskBase;

    // Slides the window forward one cell: drop the oldest digit, append the type
    // just placed.
    public int ShiftIn(int mask, int typeCode) => (mask % OldestDigitScale) * MaskBase + typeCode;
}
