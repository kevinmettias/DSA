using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CheckIfThereIsAValidParenthesesStringPath;

// LeetCode 2267. Check if There Is a Valid Parentheses String Path: walking only
// right and down from (0, 0) to (rows-1, cols-1), is there a path whose characters
// spell a balanced parenthesis string?
//
// A running integer balance is enough state: '(' adds one, ')' subtracts one, a
// balance that goes negative kills the branch immediately (an unmatched ')' can
// never be repaired by anything after it), and the bottom-right cell accepts only at
// balance zero. So a cell alone does not determine the answer - (Row, Col, Balance)
// does, and that triple is the DP state both strategies below explore.
//
// The strategies differ only in whether that state space is cached. Plain recursion
// re-derives every shared state once per path reaching it - up to C(2n-2, n-1) leaf
// calls on an n x n grid - while this repo's Memoizer collapses it to the
// polynomial number of distinct states. The induced state graph satisfies Memoizer's
// well-foundedness precondition by construction: Row + Col strictly increases on
// every recursive call, so no state can recur on its own path.
internal static class CheckIfThereIsAValidParenthesesStringPathSolution
{
    // The textbook answer: bare right/down recursion over the same states, with no
    // cache at all. Deliberately written without this repo's primitives - it is the
    // arm the memoized strategy has to justify itself against.
    public static bool HasValidPathByUnmemoizedRecursion(char[,] grid) =>
        HasValidPathFrom(0, 0, 0, Board.Over(grid));

    private static bool HasValidPathFrom(int row, int col, int balance, Board board)
    {
        var (isTerminal, terminalValue, newBalance) = board.Enter(row, col, balance);

        if (isTerminal)
        {
            return terminalValue;
        }

        var canGoDown = row + 1 < board.Rows && HasValidPathFrom(row + 1, col, newBalance, board);
        var canGoRight = col + 1 < board.Cols && HasValidPathFrom(row, col + 1, newBalance, board);
        return canGoDown || canGoRight;
    }

    // The same recurrence handed to Memoizer, whose call-back parameter routes every
    // recursive branch through one shared cache keyed by the (Row, Col, Balance)
    // state - the tuple's structural equality is exactly the identity the recurrence
    // needs, so the default comparer is correct as-is.
    public static bool HasValidPathByMemoizedRecursion(char[,] grid)
    {
        var board = Board.Over(grid);

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
        var (isTerminal, terminalValue, newBalance) = board.Enter(row, col, balance);

        if (isTerminal)
        {
            return terminalValue;
        }

        var canGoDown = row + 1 < board.Rows && hasValidPath((row + 1, col, newBalance));
        var canGoRight = col + 1 < board.Cols && hasValidPath((row, col + 1, newBalance));
        return canGoDown || canGoRight;
    }

    // The grid plus its own dimensions, and the one rule both walks share: entering a
    // cell either ends the branch (balance went negative, or the destination was
    // reached) or yields the balance to carry on with. Neither answer recurses - only
    // the caller knows how, which is the whole difference between the two strategies.
    private readonly record struct Board(char[,] Grid, int Rows, int Cols)
    {
        private const char Open = '(';

        public static Board Over(char[,] grid) => new(grid, grid.GetLength(0), grid.GetLength(1));

        public (bool IsTerminal, bool TerminalValue, int NewBalance) Enter(int row, int col, int balance)
        {
            var newBalance = balance + (Grid[row, col] == Open ? 1 : -1);

            if (newBalance < 0)
            {
                return (true, false, newBalance);
            }

            if (row == Rows - 1 && col == Cols - 1)
            {
                return (true, newBalance == 0, newBalance);
            }

            return (false, false, newBalance);
        }
    }
}
