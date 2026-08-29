using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSudoku;

public sealed partial class ValidSudokuTests
{
    [Fact]
    public void IsValidSudoku_ValidLeetCodeBoard_ReturnsTrue()
    {
        char[][] board = [
            ['5','3','.','.','7','.','.','.','.'], ['6','.','.','1','9','5','.','.','.'], ['.','9','8','.','.','.','.','6','.'],
            ['8','.','.','.','6','.','.','.','3'], ['4','.','.','8','.','3','.','.','1'], ['7','.','.','.','2','.','.','.','6'],
            ['.','6','.','.','.','.','2','8','.'], ['.','.','.','4','1','9','.','.','5'], ['.','.','.','.','8','.','.','7','9']];
        Assert.True(IsValidSudoku(board));
    }

    [Fact]
    public void IsValidSudoku_DuplicateInColumn_ReturnsFalse()
    {
        char[][] board = [
            ['8','3','.','.','7','.','.','.','.'], ['6','.','.','1','9','5','.','.','.'], ['.','9','8','.','.','.','.','6','.'],
            ['8','.','.','.','6','.','.','.','3'], ['4','.','.','8','.','3','.','.','1'], ['7','.','.','.','2','.','.','.','6'],
            ['.','6','.','.','.','.','2','8','.'], ['.','.','.','4','1','9','.','.','5'], ['.','.','.','.','8','.','.','7','9']];
        Assert.False(IsValidSudoku(board));
    }

    private static bool IsValidSudoku(char[][] board)
    {
        var seen = new Set<string>();
        for (var row = 0; row < 9; row++)
        for (var col = 0; col < 9; col++)
        {
            var digit = board[row][col];
            if (digit == '.') continue;
            if (!seen.TryAdd($"r{row}:{digit}") || !seen.TryAdd($"c{col}:{digit}") || !seen.TryAdd($"b{row / 3},{col / 3}:{digit}")) return false;
        }
        return true;
    }
}
