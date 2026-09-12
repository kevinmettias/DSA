using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CherryPickup;

// LeetCode 741. Cherry Pickup: the round trip (0,0)->(n-1,n-1)->(0,0) reframes as two
// people walking (0,0)->(n-1,n-1) simultaneously, right/down only - a forward walk
// and its reverse cover the same cells either way. The recurrence tracks
// (Row1, Col1, Col2) - Row2 is derived as Row1+Col1-Col2, since both paths always
// take the same number of steps - the same grid recurrence UniquePaths/
// LongestIncreasingPathInAMatrix already use, closed over four simultaneous move
// combinations instead of one.
//
// Both strategies walk the same recurrence over the same grid; they differ only in
// whether repeated (Row1, Col1, Col2) sub-states are cached. The induced state graph
// is well-founded for Memoizer's own precondition: Row1+Col1 strictly increases on
// every recursive call, so no state can ever recur on its own path.
internal static class CherryPickupSolution
{
    private const int Blocked = int.MinValue;

    // The textbook answer: plain exponential recursion over (Row1, Col1, Col2)
    // states, no caching - up to 4^(2n-2) calls, since the same shared state recurs
    // across many different move orderings. Deliberately written without this
    // repo's primitives; it is the arm the composed solution below has to justify
    // itself against.
    public static int MaxCherriesByUnmemoizedRecursion(int[,] grid)
    {
        var n = grid.GetLength(0);
        var cherries = CherriesFromUnmemoized(0, 0, 0, grid, n);
        return Math.Max(0, cherries);
    }

    private static int CherriesFromUnmemoized(int row1, int col1, int col2, int[,] grid, int n)
    {
        var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2, grid, n);

        if (isTerminal)
        {
            return terminalValue;
        }

        var moveDownAddCol2 = CherriesFromUnmemoized(row1 + 1, col1, col2 + 1, grid, n);
        var moveDownSameCol2 = CherriesFromUnmemoized(row1 + 1, col1, col2, grid, n);
        var moveRightAddCol2 = CherriesFromUnmemoized(row1, col1 + 1, col2 + 1, grid, n);
        var moveRightSameCol2 = CherriesFromUnmemoized(row1, col1 + 1, col2, grid, n);
        var bestNext = BestOfFour(moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

        return bestNext == Blocked ? Blocked : picked + bestNext;
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (Row1, Col1, Col2) state - collapsing the exponential walk above to the
    // polynomial n^3 distinct states (BurstBalloonsSolution precedent for this same
    // un-memoized-vs-Memoizer shape).
    public static int MaxCherriesByMemoizedRecursion(int[,] grid)
    {
        var n = grid.GetLength(0);

        var result = Memoizer.Memoize<(int Row1, int Col1, int Col2), int>(
            (0, 0, 0),
            (state, cherriesFrom) => CherriesFromMemoized(state, cherriesFrom, grid, n));

        return Math.Max(0, result);
    }

    private static int CherriesFromMemoized(
        (int Row1, int Col1, int Col2) state,
        Func<(int Row1, int Col1, int Col2), int> cherriesFrom,
        int[,] grid,
        int n)
    {
        var (row1, col1, col2) = state;
        var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2, grid, n);

        if (isTerminal)
        {
            return terminalValue;
        }

        var moveDownAddCol2 = cherriesFrom((row1 + 1, col1, col2 + 1));
        var moveDownSameCol2 = cherriesFrom((row1 + 1, col1, col2));
        var moveRightAddCol2 = cherriesFrom((row1, col1 + 1, col2 + 1));
        var moveRightSameCol2 = cherriesFrom((row1, col1 + 1, col2));
        var bestNext = BestOfFour(moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

        return bestNext == Blocked ? Blocked : picked + bestNext;
    }

    // Shared shape between the un-memoized and memoized walks: given a state, decide
    // whether it's a terminal (blocked/goal) value, and if not, the cherries picked
    // up by entering it. Neither branch here recurses - only the caller knows how.
    private static (bool IsTerminal, int TerminalValue, int Picked) EvaluateCherryState(
        int row1, int col1, int col2, int[,] grid, int n)
    {
        var row2 = row1 + col1 - col2;

        if (row1 >= n || col1 >= n || row2 < 0 || row2 >= n || col2 < 0 || col2 >= n
            || grid[row1, col1] == -1 || grid[row2, col2] == -1)
        {
            return (true, Blocked, 0);
        }

        if (row1 == n - 1 && col1 == n - 1)
        {
            return (true, grid[row1, col1], 0);
        }

        var picked = grid[row1, col1] + (col1 == col2 ? 0 : grid[row2, col2]);
        return (false, 0, picked);
    }

    private static int BestOfFour(int downAddCol2, int downSameCol2, int rightAddCol2, int rightSameCol2)
    {
        var downBest = Math.Max(downAddCol2, downSameCol2);
        var rightBest = Math.Max(rightAddCol2, rightSameCol2);
        return Math.Max(downBest, rightBest);
    }
}
