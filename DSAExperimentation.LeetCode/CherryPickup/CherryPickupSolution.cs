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
        var cherries = CherriesFromUnmemoized(0, 0, 0, grid);
        return Math.Max(0, cherries);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (Row1, Col1, Col2) state - collapsing the exponential walk above to the
    // polynomial n^3 distinct states (BurstBalloonsSolution precedent for this same
    // un-memoized-vs-Memoizer shape).
    public static int MaxCherriesByMemoizedRecursion(int[,] grid)
    {
        var result = Memoizer.Memoize<(int Row1, int Col1, int Col2), int>(
            (0, 0, 0), new CherriesFromMemoized(grid));

        return Math.Max(0, result);
    }

    // The memoized rule, named: what a (Row1, Col1, Col2) state is worth is the
    // cherries entering it plus the best of its four simultaneous forward moves.
    private sealed class CherriesFromMemoized(int[,] grid)
        : IRecurrence<(int Row1, int Col1, int Col2), int>
    {
        public int Replay(
            (int Row1, int Col1, int Col2) state,
            IRecurrence<(int Row1, int Col1, int Col2), int> rest)
        {
            var (row1, col1, col2) = state;
            var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2, grid);

            if (isTerminal)
            {
                return terminalValue;
            }

            var moveDownAddCol2 = rest.Replay((row1 + 1, col1, col2 + 1), rest);
            var moveDownSameCol2 = rest.Replay((row1 + 1, col1, col2), rest);
            var moveRightAddCol2 = rest.Replay((row1, col1 + 1, col2 + 1), rest);
            var moveRightSameCol2 = rest.Replay((row1, col1 + 1, col2), rest);
            var bestNext = BestOfFour(
                moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

            if (bestNext == Blocked)
            {
                return Blocked;
            }

            return TotalCherries(picked, bestNext);
        }
    }

    private static int CherriesFromUnmemoized(int row1, int col1, int col2, int[,] grid)
    {
        var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2, grid);

        if (isTerminal)
        {
            return terminalValue;
        }

        var moveDownAddCol2 = CherriesFromUnmemoized(row1 + 1, col1, col2 + 1, grid);
        var moveDownSameCol2 = CherriesFromUnmemoized(row1 + 1, col1, col2, grid);
        var moveRightAddCol2 = CherriesFromUnmemoized(row1, col1 + 1, col2 + 1, grid);
        var moveRightSameCol2 = CherriesFromUnmemoized(row1, col1 + 1, col2, grid);
        var bestNext = BestOfFour(moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

        return bestNext == Blocked ? Blocked : TotalCherries(picked, bestNext);
    }

    // Shared shape between the un-memoized and memoized walks: given a state, decide
    // whether it's a terminal (blocked/goal) value, and if not, the cherries picked
    // up by entering it. Neither branch here recurses - only the caller knows how.
    private static (bool IsTerminal, int TerminalValue, int Picked) EvaluateCherryState(
        int row1, int col1, int col2, int[,] grid)
    {
        var n = grid.GetLength(0);
        var row2 = row1 + col1 - col2;

        if (IsUnusablePosition(grid, row1, col1, n) || IsUnusablePosition(grid, row2, col2, n))
        {
            return (true, Blocked, 0);
        }

        if (row1 == n - 1 && col1 == n - 1)
        {
            return (true, grid[row1, col1], 0);
        }

        return (false, 0, CherriesPickedByEntering(grid, (row1, col1, col2)));
    }

    // What a non-terminal state has already banked before either walker moves on: the cherries
    // in both walkers' cells, with a shared cell counted once. Row2 is derived here rather than
    // passed in, being the same Row1+Col1-Col2 identity the recurrence itself is stated over.
    private static int CherriesPickedByEntering(int[,] grid, (int Row1, int Col1, int Col2) state)
    {
        var row2 = state.Row1 + state.Col1 - state.Col2;
        return grid[state.Row1, state.Col1]
            + (state.Col1 == state.Col2 ? 0 : CherriesAt(grid, row2, state.Col2));
    }

    // A walker's cell is unusable when it falls outside the grid or holds a thorn -
    // said once for each of the two walkers rather than eight times in one line.
    private static bool IsUnusablePosition(int[,] grid, int row, int col, int n)
        => row < 0 || row >= n || col < 0 || col >= n || grid[row, col] == -1;

    private static int BestOfFour(int downAddCol2, int downSameCol2, int rightAddCol2, int rightSameCol2)
    {
        var downBest = Math.Max(downAddCol2, downSameCol2);
        var rightBest = Math.Max(rightAddCol2, rightSameCol2);
        return Math.Max(downBest, rightBest);
    }

    // What a non-terminal state is worth: the cherries entering the cells picks up, plus
    // the best the two walkers can still collect from there.
    private static int TotalCherries(int picked, int bestNext) => picked + bestNext;

    private static int CherriesAt(int[,] grid, int row, int col) => grid[row, col];
}
