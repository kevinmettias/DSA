using DSAExperimentation.LeetCode.ValidSudoku;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSudoku;

// Harness only. Both strategies are ValidSudokuSolution's; this file just
// pins them to LeetCode's published examples.
public sealed class ValidSudokuTests
{
    public static TheoryData<SudokuBoardExample> Examples =>
        new()
        {
            new SudokuBoardExample(
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
                ],
                IsValid: true),
            new SudokuBoardExample(
                [
                    ['8', '3', '.', '.', '7', '.', '.', '.', '.'],
                    ['6', '.', '.', '1', '9', '5', '.', '.', '.'],
                    ['.', '9', '8', '.', '.', '.', '.', '6', '.'],
                    ['8', '.', '.', '.', '6', '.', '.', '.', '3'],
                    ['4', '.', '.', '8', '.', '3', '.', '.', '1'],
                    ['7', '.', '.', '.', '2', '.', '.', '.', '6'],
                    ['.', '6', '.', '.', '.', '.', '2', '8', '.'],
                    ['.', '.', '.', '4', '1', '9', '.', '.', '5'],
                    ['.', '.', '.', '.', '8', '.', '.', '7', '9'],
                ],
                IsValid: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBooleanGrid_LeetCodeExamples_ReturnsExpectedValidity(SudokuBoardExample example) =>
        Assert.Equal(example.IsValid, ValidSudokuSolution.IsValidByBooleanGrid(example.Board));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidBySetKeys_LeetCodeExamples_ReturnsExpectedValidity(SudokuBoardExample example) =>
        Assert.Equal(example.IsValid, ValidSudokuSolution.IsValidBySetKeys(example.Board));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct SudokuBoardExample(char[][] Board, bool IsValid);
}
