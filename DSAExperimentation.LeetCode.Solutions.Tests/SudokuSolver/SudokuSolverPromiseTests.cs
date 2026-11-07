using DSAExperimentation.LeetCode.Solutions.SudokuSolver;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SudokuSolver;

// Boards written a row to a string, '.' for an empty cell.
public sealed partial class SudokuSolverPromiseTests
{
    private const string EmptyRow = ".........";

    // LeetCode's own example, which has one solution.
    private static readonly string[] Example =
    [
        "53..7....", "6..195...", ".98....6.", "8...6...3", "4..8.3..1", "7...2...6", ".6....28.", "...419..5", "....8..79",
    ];

    // Every cell empty: as many solutions as there are Sudoku grids.
    private static readonly string[] Empty = [EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow];

    // Two 5s in the first row.
    private static readonly string[] RepeatedGiven = ["55.......", EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow];

    // No given repeats, but the first row's last cell can take only 9, which its column already holds.
    private static readonly string[] Unsolvable = ["12345678.", "........9", EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow, EmptyRow];

    public static TheoryData<string[]> BoardsWithoutOneSolution => new() { Empty, RepeatedGiven, Unsolvable };

    [Fact]
    public void IsKeptBy_BoardWithOneSolution_IsTrue() => Assert.True(SudokuSolverPromise.IsKeptBy(BoardOf(Example)));

    [Theory]
    [MemberData(nameof(BoardsWithoutOneSolution))]
    public void IsKeptBy_BoardWithNoneOrMany_IsFalse(string[] rows) => Assert.False(SudokuSolverPromise.IsKeptBy(BoardOf(rows)));

    [Fact]
    public void IsKeptBy_AnyBoard_LeavesItAsItWas()
    {
        var board = BoardOf(Example);

        SudokuSolverPromise.IsKeptBy(board);

        Assert.Equal(Example, board.Select(row => new string(row)));
    }

    private static char[][] BoardOf(string[] rows) => [.. rows.Select(row => row.ToCharArray())];
}
