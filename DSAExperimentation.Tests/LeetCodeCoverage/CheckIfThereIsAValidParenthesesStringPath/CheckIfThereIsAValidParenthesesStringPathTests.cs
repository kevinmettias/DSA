using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidParenthesesStringPath;

// LeetCode 2267. Check if There Is a Valid Parentheses String Path: the same
// Memoizer-over-(Row,Col,...)-state recurrence CherryPickupTests already proves out
// for a grid DP, closed over a running parenthesis balance instead of a cherry
// total. Balance going negative prunes the branch immediately (mirrors
// ValidParenthesisStringTests' "an unmatched ')' with nothing left to pair against
// fails fast" rule, just re-expressed as an integer instead of a stack pop); reaching
// (rows-1, cols-1) with balance zero is the only accepting state. The induced state
// graph is well-founded for Memoizer's own precondition: Row+Col strictly increases
// on every recursive call, so no state can ever recur on its own path.
public sealed partial class CheckIfThereIsAValidParenthesesStringPathTests
{
    [Fact]
    public void HasValidPath_ARightThenDownPathBalances_ReturnsTrue()
    {
        char[,] grid =
        {
            { '(', '(', ')' },
            { '(', ')', ')' },
        };

        Assert.True(HasValidPath(grid));
    }

    [Fact]
    public void HasValidPath_StartsWithAClosingParenthesis_ReturnsFalse()
    {
        char[,] grid =
        {
            { ')', '(', ')' },
            { '(', ')', ')' },
        };

        Assert.False(HasValidPath(grid));
    }

    [Fact]
    public void HasValidPath_SingleCellCanNeverBalance_ReturnsFalse()
    {
        char[,] grid = { { '(' } };

        Assert.False(HasValidPath(grid));
    }

    private static bool HasValidPath(char[,] grid)
    {
        var board = new Board(grid, grid.GetLength(0), grid.GetLength(1));

        return Memoizer.Memoize<(int Row, int Col, int Balance), bool>(
            (0, 0, 0),
            (state, hasValidPath) => HasValidPathFrom(state, hasValidPath, board));
    }

    private static bool HasValidPathFrom(
        (int Row, int Col, int Balance) state,
        Func<(int Row, int Col, int Balance), bool> hasValidPath,
        Board board)
    {
        var (row, col, balance) = state;
        balance += board.Grid[row, col] == '(' ? 1 : -1;

        if (balance < 0)
        {
            return false;
        }

        if (row == board.Rows - 1 && col == board.Cols - 1)
        {
            return balance == 0;
        }

        var canGoDown = row + 1 < board.Rows && hasValidPath((row + 1, col, balance));
        var canGoRight = col + 1 < board.Cols && hasValidPath((row, col + 1, balance));
        return canGoDown || canGoRight;
    }

    private readonly record struct Board(char[,] Grid, int Rows, int Cols);
}
