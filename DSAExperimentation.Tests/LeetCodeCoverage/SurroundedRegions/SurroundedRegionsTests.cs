using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SurroundedRegions;

public sealed partial class SurroundedRegionsTests
{
    [Fact]
    public void Solve_ClassicExample_CapturesInteriorRegions()
    {
        char[][] board = [['X','X','X','X'],['X','O','O','X'],['X','X','O','X'],['X','O','X','X']];
        Solve(board);
        Assert.Equal([['X','X','X','X'],['X','X','X','X'],['X','X','X','X'],['X','O','X','X']], board);
    }

    private static void Solve(char[][] board)
    {
        var rows = board.Length; var cols = board[0].Length;
        for (var r = 0; r < rows; r++) { Mark((r, 0)); Mark((r, cols - 1)); }
        for (var c = 0; c < cols; c++) { Mark((0, c)); Mark((rows - 1, c)); }
        for (var r = 0; r < rows; r++) for (var c = 0; c < cols; c++) board[r][c] = board[r][c] == '#' ? 'O' : 'X';
        void Mark((int Row, int Col) start)
        {
            if (board[start.Row][start.Col] != 'O') return;
            foreach (var (row, col) in DepthFirstSearch.Traverse(start, Neighbors)) board[row][col] = '#';
        }
        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            (int Row, int Col)[] next = [(p.Row + 1, p.Col), (p.Row - 1, p.Col), (p.Row, p.Col + 1), (p.Row, p.Col - 1)];
            foreach (var n in next) if (n.Row >= 0 && n.Row < rows && n.Col >= 0 && n.Col < cols && board[n.Row][n.Col] == 'O') yield return n;
        }
    }
}
