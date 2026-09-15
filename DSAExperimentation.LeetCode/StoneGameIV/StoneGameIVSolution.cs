using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGameIV;

// LeetCode 1510. Stone Game IV: players alternate removing a non-zero square number
// of stones from a pile of n; whoever cannot move loses. Alice wins iff some perfect
// square x (1 <= x*x <= n) leaves Bob facing a losing position - the same minimax
// recurrence DivisorGameSolution and StoneGameIIISolution already state.
//
// Both strategies below are that one recurrence; they differ only in whether the
// remaining-stone count is cached. Keeping them in one class is what puts the
// un-memoized arm under the same assertions as the memoized one - it used to exist
// only as StoneGameIVBenchmarks' baseline, measured but never asserted.
internal static class StoneGameIVSolution
{
    // The textbook answer: plain recursion over the remaining count, no cache.
    // Exponential, because the same remaining count recurs through many different
    // square-removal sequences that reach it. Deliberately written with nothing but
    // the call stack - it is the arm the memoized strategy has to justify itself
    // against.
    public static bool AliceWinsByUnmemoizedRecursion(int n)
    {
        for (var square = 1; square * square <= n; square++)
        {
            if (!AliceWinsByUnmemoizedRecursion(n - (square * square)))
            {
                return true;
            }
        }

        return false;
    }

    // The same recurrence over this repo's own Memoizer, which caches each remaining
    // count the first time it is resolved, collapsing the exponential tree to one
    // evaluation per distinct count.
    public static bool AliceWinsByMemoizedRecursion(int n)
        => Memoizer.Memoize<int, bool>(n, new WinFromSquareRemoval());

    // The recurrence, as a named type: the mover wins from `state` stones exactly when
    // some square-sized removal leaves the opponent losing, and loses once no square
    // still fits. This is the decision the bare lambda left anonymous.
    private sealed class WinFromSquareRemoval : IRecurrence<int, bool>
    {
        public bool Replay(int state, IRecurrence<int, bool> rest)
        {
            for (var square = 1; square * square <= state; square++)
            {
                if (!rest.Replay(state - (square * square), rest))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
