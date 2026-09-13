using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DivisorGame;

// LeetCode 1025. Divisor Game: Alice moves first and must replace n with n - x for
// some divisor x of n with 0 < x < n; a player who cannot move loses. Alice wins iff
// some legal move hands Bob a losing position.
//
// AliceWinsByMemoizedRecursion states that definition literally - the natural-looking
// game-theory recursion over this repo's own Memoizer, the same shape NimGame and
// StoneGame use for their recurrences. AliceWinsByParityFormula is the O(1) closed
// form the recursion provably reduces to ("n is even"), which is the arm the
// recursion is measured against; keeping both here is what finally puts them under
// the same assertions instead of one living in a test and one in a benchmark.
internal static class DivisorGameSolution
{
    private const int ParityDivisor = 2;

    // O(n^2) memoized search: for each state, scan every candidate divisor x and win
    // as soon as one leaves the opponent in a losing state.
    public static bool AliceWinsByMemoizedRecursion(int n)
        => Memoizer.Memoize<int, bool>(n, (current, aliceWins) =>
        {
            for (var x = 1; x < current; x++)
            {
                if (current % x == 0 && !aliceWins(current - x))
                {
                    return true;
                }
            }

            return false;
        });

    // The closed form: subtracting 1 always flips parity, and from an odd n every
    // divisor is odd, so odd positions can only hand back even ones. Alice wins
    // exactly when n is even.
    public static bool AliceWinsByParityFormula(int n) => n % ParityDivisor == 0;
}
