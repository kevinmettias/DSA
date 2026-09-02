using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.IntegerReplacement;

// LeetCode 397. Integer Replacement: fewest operations to reduce n to 1, where an
// even n may only be halved and an odd n may only be incremented or decremented -
// replace(n) = 0 when n == 1, 1 + replace(n/2) when n is even, and
// 1 + min(replace(n-1), replace(n+1)) when n is odd.
//
// Both strategies walk that exact recurrence; the only difference is whether
// repeated states are cached. Recursion runs over long, not int, even though
// LeetCode's own n is an int: n can be int.MaxValue (2^31 - 1), and the n+1 branch
// on the odd case would otherwise silently overflow Int32.
internal static class IntegerReplacementSolution
{
    private const int EvenCheckDivisor = 2;

    // The textbook answer: plain unmemoized recursion. On a repeating-bit input
    // (0b0101...01) every bit position forces the odd branch, so this arm's call
    // tree explodes into millions of redundant calls - the arm the memoized
    // recurrence below has to justify itself against.
    public static int MinStepsByUnmemoizedRecursion(int n) => Replace(n);

    private static int Replace(long n)
        => n switch
        {
            1 => 0,
            _ when n % EvenCheckDivisor == 0 => 1 + Replace(n / EvenCheckDivisor),
            _ => 1 + Math.Min(Replace(n - 1), Replace(n + 1)),
        };

    // This repo's own Memoizer-driven DP recurrence (same CountingBitsSolution/
    // HouseRobberSolution composition) over the identical recurrence above - caching
    // collapses the same exploding call tree down to a few dozen distinct states.
    public static int MinStepsByMemoizedRecurrence(int n)
        => Memoizer.Memoize<long, int>(n, (value, replace) => value switch
        {
            1 => 0,
            _ when value % EvenCheckDivisor == 0 => 1 + replace(value / EvenCheckDivisor),
            _ => 1 + Math.Min(replace(value - 1), replace(value + 1)),
        });
}
