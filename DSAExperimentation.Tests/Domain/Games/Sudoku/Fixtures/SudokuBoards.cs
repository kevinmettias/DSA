namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// Boards the exact cover tests solve, a row to a string, '.' for an empty cell: LeetCode 37's
// example puzzle, and solved boards of the other grids, which a test empties as a checkerboard
// to pose a puzzle.
internal static class SudokuBoards
{
    // LC 37's example, which has one solution.
    public static string[] LeetCodeExample { get; } =
    [
        "53..7....", "6..195...", ".98....6.", "8...6...3", "4..8.3..1", "7...2...6", ".6....28.",
        "...419..5", "....8..79",
    ];

    public static string[] SolvedSixBySix { get; } =
        ["123456", "456123", "234561", "561234", "345612", "612345"];

    // Written in 0 to F.
    public static string[] SolvedSixteenBySixteen { get; } =
    [
        "0123456789ABCDEF", "456789ABCDEF0123", "89ABCDEF01234567", "CDEF0123456789AB",
        "123456789ABCDEF0", "56789ABCDEF01234", "9ABCDEF012345678", "DEF0123456789ABC",
        "23456789ABCDEF01", "6789ABCDEF012345", "ABCDEF0123456789", "EF0123456789ABCD",
        "3456789ABCDEF012", "789ABCDEF0123456", "BCDEF0123456789A", "F0123456789ABCDE",
    ];

    // The 5x5 pentomino jigsaw's (PentominoJigsaw).
    public static string[] SolvedJigsaw { get; } = ["12345", "41523", "34251", "25134", "53412"];
}
