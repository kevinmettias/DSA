namespace DSAExperimentation.LeetCode.StoneGameIV;

// LeetCode 1510. Stone Game IV: players alternate removing a non-zero square number
// of stones from a pile of n; whoever cannot move loses. Alice wins iff some perfect
// square x (1 <= x*x <= n) leaves Bob facing a losing position - the same minimax
// recurrence DivisorGameSolution and StoneGameIIISolution already state.
//
// Both strategies below are that one recurrence. The baseline recurses on it with no
// cache; the table settles every count from 0 up to n in one ascending pass, with no
// recursion at all, so LC 1510's n up to 10^5 costs it nothing in stack depth - the
// memoized recursion it replaces went one call deeper per stone and overflowed the
// stack at n = 10,000.
internal static class StoneGameIVSolution
{
    // The textbook answer: plain recursion over the remaining count, no cache.
    // Exponential, because the same remaining count recurs through many different
    // square-removal sequences that reach it. Deliberately written with nothing but
    // the call stack - it is the arm the table has to justify itself against.
    public static bool CanAliceWinByUnmemoizedRecursion(int stoneCount)
    {
        for (var square = 1; square * square <= stoneCount; square++)
        {
            if (!CanAliceWinByUnmemoizedRecursion(stoneCount - (square * square)))
            {
                return true;
            }
        }

        return false;
    }

    // Bottom-up over the stone counts, pushing rather than pulling: a losing count
    // marks every count one square above it as a win for the player to move. When the
    // pass reaches a count, every smaller count has already pushed into it, so its
    // entry is final - a win exactly when some square below it lands on a loss, the
    // recurrence above. Only losing counts push, and they thin out: 2,781 of the
    // 100,001 counts up to 10^5 lose, so the pass makes about 641,000 writes where
    // pulling - each count probing squares down until it finds a loss - makes about
    // 4.9 million probes (both counted by a separate script). The bound is O(n + L sqrt n)
    // for L losing counts up to n, so O(n sqrt n) at worst, over one n + 1 entry table.
    public static bool CanAliceWinByBottomUpTable(int stoneCount)
    {
        var moverWins = new bool[stoneCount + 1];

        for (var count = 0; count < stoneCount; count++)
        {
            if (!moverWins[count])
            {
                MarkCountsOneSquareAbove(moverWins, count);
            }
        }

        return moverWins[stoneCount];
    }

    // A move from any count one square above a losing count reaches it, so the player
    // to move there wins.
    private static void MarkCountsOneSquareAbove(bool[] moverWins, int losingCount)
    {
        for (var square = 1; losingCount + (square * square) < moverWins.Length; square++)
        {
            moverWins[losingCount + (square * square)] = true;
        }
    }
}
