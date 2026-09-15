using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.ValidSudoku;

// LeetCode 36. Valid Sudoku: determine whether a partially filled 9x9 board
// has no duplicate digit in any row, column, or 3x3 box ('.' is empty and
// never conflicts).
//
// Both strategies visit every filled cell once and record which (row, col,
// box) unit a digit already occupies; they differ only in what tracks
// "already seen" - three dense bool grids sized to the fixed 9-cell axes, or
// this repo's own Set<string> keyed by a composed row/col/box tag.
internal static class ValidSudokuSolution
{
    private const int BoardSize = 9;
    private const int DigitRange = 10;
    private const int BoxSize = 3;

    // The textbook answer: three dense bool[,] grids, one per constraint
    // axis, indexed by [unit, digit]. Deliberately written without this
    // repo's primitives - it is the arm the Set-keyed strategy has to
    // justify itself against.
    public static bool IsValidByBooleanGrid(char[][] board)
    {
        var units = (
            Rows: new bool[BoardSize, DigitRange],
            Cols: new bool[BoardSize, DigitRange],
            Boxes: new bool[BoardSize, DigitRange]);

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                if (!TryRecordCell(board, units, row, col))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // One cell's whole rule: an empty cell records nothing, a digit that any of its three
    // units already holds is the conflict that ends the scan, and otherwise the digit is
    // written to the row, the column, and the 3x3 box at once. The three grids travel as
    // one tracker because every digit occupies all three of them together.
    private static bool TryRecordCell(
        char[][] board,
        (bool[,] Rows, bool[,] Cols, bool[,] Boxes) units,
        int row,
        int col)
    {
        var digit = board[row][col];

        if (digit == '.')
        {
            return true;
        }

        var value = digit - '0';
        var box = (row / BoxSize * BoxSize) + (col / BoxSize);

        if (IsDigitAlreadyPlaced(units, (row, col, box, value)))
        {
            return false;
        }

        units.Rows[row, value] = units.Cols[col, value] = units.Boxes[box, value] = true;
        return true;
    }

    // A digit is already placed when the row, the column, or the 3x3 box it would go into
    // has seen it before - the three units the rules constrain at once.
    private static bool IsDigitAlreadyPlaced(
        (bool[,] Rows, bool[,] Cols, bool[,] Boxes) units,
        (int Row, int Col, int Box, int Value) placement) =>
        units.Rows[placement.Row, placement.Value] ||
        units.Cols[placement.Col, placement.Value] ||
        units.Boxes[placement.Box, placement.Value];

    // Track "already seen" with one Set<string> keyed by a composed
    // row/col/box tag rather than three parallel grids - each TryAdd both
    // checks and records in a single call.
    public static bool IsValidBySetKeys(char[][] board)
    {
        var seen = new Set<string>();

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                var digit = board[row][col];
                if (digit == '.')
                {
                    continue;
                }

                if (!TryRecordDigit(seen, row, col, digit))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Records the digit in all three of its units, stopping at the first that already held
    // it. Returns whether the whole cell could be recorded; the units the digit reached
    // before the conflict stay recorded, exactly as the three TryAdd calls did.
    private static bool TryRecordDigit(Set<string> seen, int row, int col, char digit) =>
        seen.TryAdd($"r{row}:{digit}") &&
        seen.TryAdd($"c{col}:{digit}") &&
        seen.TryAdd($"b{row / BoxSize},{col / BoxSize}:{digit}");
}
