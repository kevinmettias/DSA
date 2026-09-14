using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfDistinctRollSequences;

// LeetCode 2318. Number of Distinct Roll Sequences: count the length-n sequences of
// die rolls where consecutive rolls are coprime and equal values sit at least three
// positions apart, modulo 1e9+7.
//
// Both strategies walk the same recurrence over the state (day, secondLastRoll,
// lastRoll) - the "gap of at least 2" rule collapses to exactly "differs from the
// previous two rolls", since any farther-apart repeat is unconstrained, so those
// three fields are the whole state. They differ only in whether that state's answer
// is remembered: the baseline re-derives it on every distinct path that reaches it
// (exponential in n), the composed arm routes it through this repo's own Memoizer
// and visits each of the O(n) reachable states once.
internal static class NumberOfDistinctRollSequencesSolution
{
    private const int DieFaces = 6;
    private const int FirstDay = 1;

    // Stands in for "no roll yet" in the first two positions. Real faces are 1-6,
    // so it can never collide with one, and the coprimality test is skipped while
    // the previous roll is still this sentinel.
    private const int NoRoll = 0;

    // The textbook answer: plain recursion, no cache. Deliberately written without
    // this repo's primitives - it is the arm the composed solution has to justify
    // itself against.
    public static long DistinctSequencesByBruteForceRecursion(int n) =>
        CountSequences(n, FirstDay, NoRoll, NoRoll);

    private static long CountSequences(int n, int day, int secondLastRoll, int lastRoll)
    {
        if (day > n)
        {
            return 1;
        }

        var total = 0L;

        for (var face = 1; face <= DieFaces; face++)
        {
            if (!CanFollow(face, secondLastRoll, lastRoll))
            {
                continue;
            }

            total = (total + CountSequences(n, day + 1, lastRoll, face)) % ModularArithmetic.Modulo;
        }

        return total;
    }

    // This repo's own answer: the identical recurrence handed to Memoizer, keyed on
    // the 3-tuple state - the same composition NumberOfMusicPlaylists and
    // SuperEggDrop use for a multi-field DP state.
    public static long DistinctSequencesByMemoizedRecursion(int n)
    {
        return Memoizer.Memoize<(int Day, int SecondLastRoll, int LastRoll), long>(
            (FirstDay, NoRoll, NoRoll),
            Ways);

        long Ways((int Day, int SecondLastRoll, int LastRoll) state, Func<(int, int, int), long> ways)
        {
            var (day, secondLastRoll, lastRoll) = state;

            if (day > n)
            {
                return 1;
            }

            var total = 0L;

            for (var face = 1; face <= DieFaces; face++)
            {
                if (!CanFollow(face, secondLastRoll, lastRoll))
                {
                    continue;
                }

                total = (total + ways((day + 1, lastRoll, face))) % ModularArithmetic.Modulo;
            }

            return total;
        }
    }

    // The problem's two rules in one place: a face may not repeat either of the
    // previous two rolls, and must be coprime with the roll immediately before it.
    private static bool CanFollow(int face, int secondLastRoll, int lastRoll)
    {
        if (face == lastRoll || face == secondLastRoll)
        {
            return false;
        }

        return lastRoll == NoRoll || Gcd(face, lastRoll) == 1;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
