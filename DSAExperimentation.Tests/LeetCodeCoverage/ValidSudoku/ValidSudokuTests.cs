using DSAExperimentation.LeetCode.ValidSudoku;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSudoku;

// Harness only. Both strategies are ValidSudokuSolution's; this file just
// pins them to LeetCode's published examples.
public sealed class ValidSudokuTests
{
    public static TheoryData<char[][], bool> Examples =>
        new()
        {
            {
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
                true
            },
            {
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
                false
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBooleanGrid_LeetCodeExamples_ReturnsExpectedValidity(char[][] board, bool expected) =>
        Assert.Equal(expected, ValidSudokuSolution.IsValidByBooleanGrid(board));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidBySetKeys_LeetCodeExamples_ReturnsExpectedValidity(char[][] board, bool expected) =>
        Assert.Equal(expected, ValidSudokuSolution.IsValidBySetKeys(board));
}
