using DSAExperimentation.LeetCode.Solutions.ValidSudoku;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidSudoku;

// Harness only. Both strategies are ValidSudokuSolution's. CatalogCaseTests holds them to
// LeetCode's published examples (ARCHITECTURE 17.13); this file adds boards that break exactly
// one unit, which no published example does.
public sealed partial class ValidSudokuSolutionTests
{
    // Example 1 with one digit added that repeats in exactly one unit - a row, a column, a box -
    // so each case fails if that one unit's check is dropped. LeetCode's own invalid example
    // repeats its 8 in a column and a box at once, which leaves any single check unguarded.
    public static TheoryData<SudokuBoardExample> ConflictsInExactlyOneUnit =>
        new()
        {
            new SudokuBoardExample(ExampleOneWith(0, 6, '5'), IsValid: false),
            new SudokuBoardExample(ExampleOneWith(6, 0, '5'), IsValid: false),
            new SudokuBoardExample(ExampleOneWith(1, 2, '3'), IsValid: false),
        };

    [Theory]
    [MemberData(nameof(ConflictsInExactlyOneUnit))]
    public void IsValidByBooleanGrid_ConflictInExactlyOneUnit_ReturnsFalse(SudokuBoardExample example) =>
        Assert.Equal(example.IsValid, ValidSudokuSolution.IsValidByBooleanGrid(example.Board));

    [Theory]
    [MemberData(nameof(ConflictsInExactlyOneUnit))]
    public void IsValidBySetKeys_ConflictInExactlyOneUnit_ReturnsFalse(SudokuBoardExample example) =>
        Assert.Equal(example.IsValid, ValidSudokuSolution.IsValidBySetKeys(example.Board));

    private static char[][] ExampleOneBoard() =>
    [
        ['5', '3', '.', '.', '7', '.', '.', '.', '.'],
        ['6', '.', '.', '1', '9', '5', '.', '.', '.'],
        ['.', '9', '8', '.', '.', '.', '.', '6', '.'],
        ['8', '.', '.', '.', '6', '.', '.', '.', '3'],
        ['4', '.', '.', '8', '.', '3', '.', '.', '1'],
        ['7', '.', '.', '.', '2', '.', '.', '.', '6'],
        ['.', '6', '.', '.', '.', '.', '2', '8', '.'],
        ['.', '.', '.', '4', '1', '9', '.', '.', '5'],
        ['.', '.', '.', '.', '8', '.', '.', '7', '9'],
    ];

    private static char[][] ExampleOneWith(int row, int col, char digit)
    {
        var board = ExampleOneBoard();
        board[row][col] = digit;
        return board;
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one board and its verdict.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct SudokuBoardExample(char[][] Board, bool IsValid);
}
