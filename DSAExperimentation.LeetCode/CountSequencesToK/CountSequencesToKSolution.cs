using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CountSequencesToK;

// LeetCode 3850. Count Sequences to K: starting at val = 1, each of nums's up to
// 19 entries (each in [1,6]) is multiplied into val, divided out of val, or
// left alone; count the choice sequences whose final rational val equals k
// exactly (division is exact rational division, not integer division).
//
// Every nums[i] factors only into the primes 2, 3 and 5, so val is always of
// the form 2^e2 * 3^e3 * 5^e5 with no other primes and never needs to be
// materialized as a fraction - the whole search reduces to walking three
// bounded exponent counters. CountSequencesByPrimeExponentMemo tracks exactly
// that (index, e2, e3, e5) tuple through this repo's own
// Algorithms.DynamicProgramming.Memoizer, the same "tuple state through
// Memoizer.Memoize" shape FindNthSmallestIntegerWithKOneBitsSolution's
// Binomial helper already uses. If k itself has a prime factor outside
// {2,3,5} no sequence can ever reach it, so the search is skipped entirely
// via TargetExponents' Reachable flag.
//
// CountSequencesByBruteForceSearch is the textbook baseline this has to
// justify itself against: recurse over all 3^n choice sequences, carrying val
// as an exact (unreduced) numerator/denominator pair of plain longs. That is
// safe from overflow without ever needing a GCD reduction - each index
// contributes at most one factor to at most one of numerator/denominator, so
// their product across any sequence never exceeds 6^19, comfortably inside
// long range.
internal static class CountSequencesToKSolution
{
    public static long CountSequencesByBruteForceSearch(int[] nums, long k) =>
        BruteForceSearch(nums, index: 0, numerator: 1, denominator: 1, k);

    private static long BruteForceSearch(int[] nums, int index, long numerator, long denominator, long k)
    {
        if (index == nums.Length)
        {
            return numerator % denominator == 0 && numerator / denominator == k ? 1 : 0;
        }

        var value = nums[index];

        return BruteForceSearch(nums, index + 1, numerator * value, denominator, k)
            + BruteForceSearch(nums, index + 1, numerator, denominator * value, k)
            + BruteForceSearch(nums, index + 1, numerator, denominator, k);
    }

    public static long CountSequencesByPrimeExponentMemo(int[] nums, long k)
    {
        var (targetE2, targetE3, targetE5, reachable) = TargetExponents(k);

        if (!reachable)
        {
            return 0;
        }

        var exponents = new (int E2, int E3, int E5)[nums.Length];
        for (var i = 0; i < nums.Length; i++)
        {
            exponents[i] = PrimeExponents(nums[i]);
        }

        return Memoizer.Memoize<(int Index, int E2, int E3, int E5), long>(
            (0, 0, 0, 0),
            (state, count) =>
            {
                var (index, e2, e3, e5) = state;

                if (index == nums.Length)
                {
                    return e2 == targetE2 && e3 == targetE3 && e5 == targetE5 ? 1 : 0;
                }

                var (d2, d3, d5) = exponents[index];

                return count((index + 1, e2 + d2, e3 + d3, e5 + d5))
                    + count((index + 1, e2 - d2, e3 - d3, e5 - d5))
                    + count((index + 1, e2, e3, e5));
            });
    }

    private static (int E2, int E3, int E5) PrimeExponents(int value) => value switch
    {
        1 => (0, 0, 0),
        2 => (1, 0, 0),
        3 => (0, 1, 0),
        4 => (2, 0, 0),
        5 => (0, 0, 1),
        6 => (1, 1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "nums[i] must be in [1,6]."),
    };

    // Strips k down to its 2/3/5 exponents; Reachable is false when what is left
    // over isn't 1, i.e. k has some other prime factor no sequence could ever
    // produce.
    private static (int E2, int E3, int E5, bool Reachable) TargetExponents(long k)
    {
        var e2 = 0;
        var e3 = 0;
        var e5 = 0;

        while (k % 2 == 0)
        {
            k /= 2;
            e2++;
        }

        while (k % 3 == 0)
        {
            k /= 3;
            e3++;
        }

        while (k % 5 == 0)
        {
            k /= 5;
            e5++;
        }

        return (e2, e3, e5, k == 1);
    }
}
