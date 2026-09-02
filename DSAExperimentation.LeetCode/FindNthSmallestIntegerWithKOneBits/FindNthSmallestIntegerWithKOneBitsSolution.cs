using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FindNthSmallestIntegerWithKOneBits;

// LeetCode 3821. Find Nth Smallest Integer With K One Bits: the n-th smallest
// (1-indexed) positive integer whose binary representation has exactly k one
// bits. The answer is guaranteed to be strictly less than 2^50.
//
// NthSmallestByPopCountScan is the textbook brute force: walk every positive
// integer in order, counting the ones whose popcount matches k, deliberately
// BCL-only (System.Numerics.BitOperations) - the arm the composed strategy
// below has to justify itself against.
//
// NthSmallestByMemoizedBinomialSelection is the combinatorial-number-system
// construction: a k-subset of bit positions has a well-defined rank among all
// k-subsets ordered by the integer they encode, so the (n-1)-th (0-indexed)
// rank can be decoded highest-bit-first - at each remaining one-bit count,
// the largest position c with Binomial(c, onesLeft) <= remaining is this
// rank's next set bit, exactly the standard "unrank a combination" recipe.
// Binomial itself is Pascal's addition rule run through this repo's own
// Memoizer, the same "tuple state through Memoizer.Memoize" shape
// ClimbingStairsSolution and MinimumSumOfValuesByDividingArraySolution
// already use.
internal static class FindNthSmallestIntegerWithKOneBitsSolution
{
    // Comfortably above the highest bit position the guarantee "answer < 2^50"
    // can ever require (bit 49), so the descending search below always starts
    // high enough without needing to special-case its first iteration.
    private const int MaxBitPosition = 59;

    public static long NthSmallestByPopCountScan(long n, int k)
    {
        var candidate = 0L;
        var found = 0L;

        while (found < n)
        {
            candidate++;
            if (BitOperations.PopCount((ulong)candidate) == k)
            {
                found++;
            }
        }

        return candidate;
    }

    public static long NthSmallestByMemoizedBinomialSelection(long n, int k)
    {
        var remaining = n - 1;
        var value = 0L;
        var searchFrom = MaxBitPosition;

        for (var onesLeft = k; onesLeft >= 1; onesLeft--)
        {
            while (Binomial(searchFrom, onesLeft) > remaining)
            {
                searchFrom--;
            }

            remaining -= Binomial(searchFrom, onesLeft);
            value |= 1L << searchFrom;
            searchFrom--;
        }

        return value;
    }

    // C(items, chosen) via Pascal's rule, memoized per call by this repo's
    // Memoizer rather than a hand-threaded cache.
    private static long Binomial(int items, int chosen) =>
        Memoizer.Memoize<(int Items, int Chosen), long>(
            (items, chosen),
            (state, choose) =>
            {
                var (n, k) = state;

                if (k == 0)
                {
                    return 1;
                }

                return n == 0 ? 0 : choose((n - 1, k - 1)) + choose((n - 1, k));
            });
}
