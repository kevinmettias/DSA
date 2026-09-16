using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FindNthSmallestIntegerWithKOneBits;

// LeetCode 3821. Find Nth Smallest Integer With K One Bits: among the positive
// integers whose binary representation has exactly oneBitCount one bits, find
// the one at the given 1-indexed position. The answer is guaranteed to be
// strictly less than 2^50.
//
// NthSmallestByPopCountScan is the textbook brute force: walk every positive
// integer in order, counting the ones whose popcount matches oneBitCount,
// deliberately BCL-only (System.Numerics.BitOperations) - the arm the composed
// strategy below has to justify itself against.
//
// NthSmallestByMemoizedBinomialSelection is the combinatorial-number-system
// construction: a subset of oneBitCount bit positions has a well-defined rank
// among all such subsets ordered by the integer they encode, so the desired
// rank - position - 1, counting from zero - can be decoded highest-bit-first:
// at each remaining one-bit count, the largest bit position c with
// Binomial(c, onesLeft) <= remaining is this rank's next set bit, exactly the
// standard "unrank a combination" recipe.
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

    public static long NthSmallestByPopCountScan(long position, int oneBitCount)
    {
        var candidate = 0L;
        var found = 0L;

        while (found < position)
        {
            candidate++;
            if (BitOperations.PopCount((ulong)candidate) == oneBitCount)
            {
                found++;
            }
        }

        return candidate;
    }

    public static long NthSmallestByMemoizedBinomialSelection(long position, int oneBitCount)
    {
        var remaining = position - 1;
        var value = 0L;
        var searchFrom = MaxBitPosition;

        for (var onesLeft = oneBitCount; onesLeft >= 1; onesLeft--)
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
        Memoizer.Memoize<(int Items, int Chosen), long>((items, chosen), new PascalAddition());

    /// <summary>
    /// The recurrence, named: C(n, k) is 1 at k = 0, 0 at n = 0, and otherwise
    /// Pascal's rule - the two terms of the row above, summed.
    /// </summary>
    private sealed class PascalAddition : IRecurrence<(int Items, int Chosen), long>
    {
        /// <inheritdoc/>
        public long Replay((int Items, int Chosen) state, IRecurrence<(int Items, int Chosen), long> rest)
        {
            var (items, chosen) = state;

            if (chosen == 0)
            {
                return 1;
            }

            if (items == 0)
            {
                return 0;
            }

            return rest.Replay((items - 1, chosen - 1), rest) + rest.Replay((items - 1, chosen), rest);
        }
    }
}
