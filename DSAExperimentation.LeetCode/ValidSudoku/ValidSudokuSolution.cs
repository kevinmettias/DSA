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
        var rows = new bool[BoardSize, DigitRange];
        var cols = new bool[BoardSize, DigitRange];
        var boxes = new bool[BoardSize, DigitRange];

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                var digit = board[row][col];
                if (digit == '.')
                {
                    continue;
                }

                var value = digit - '0';
                var box = (row / BoxSize * BoxSize) + (col / BoxSize);

                if (rows[row, value] || cols[col, value] || boxes[box, value])
                {
                    return false;
                }

                rows[row, value] = cols[col, value] = boxes[box, value] = true;
            }
        }

        return true;
    }

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

                if (!seen.TryAdd($"r{row}:{digit}") ||
                    !seen.TryAdd($"c{col}:{digit}") ||
                    !seen.TryAdd($"b{row / BoxSize},{col / BoxSize}:{digit}"))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
