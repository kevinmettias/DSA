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
        BruteForceSearch(nums, index: 0, rational: (1, 1), k);

    public static long CountSequencesByPrimeExponentMemo(int[] nums, long k)
    {
        var (targetE2, targetE3, targetE5, reachable) = TargetExponents(k);

        if (!reachable)
        {
            return 0;
        }

        return CountSequencesByMemoSearch(nums, (targetE2, targetE3, targetE5));
    }

    // Strips k down to its 2/3/5 exponents; Reachable is false when what is left
    // over isn't 1, i.e. k has some other prime factor no sequence could ever
    // produce.
    private static (int E2, int E3, int E5, bool Reachable) TargetExponents(long k)
    {
        var (after2, e2) = ExtractPrimeFactor(k, 2);
        var (after3, e3) = ExtractPrimeFactor(after2, 3);
        var (after5, e5) = ExtractPrimeFactor(after3, 5);

        return (e2, e3, e5, after5 == 1);
    }

    // Divides one prime out of value as many times as it goes, handing back what is
    // left together with the exponent it yielded.
    private static (long Remaining, int Exponent) ExtractPrimeFactor(long value, int prime)
    {
        var exponent = 0;

        while (value % prime == 0)
        {
            value /= prime;
            exponent++;
        }

        return (value, exponent);
    }

    // Walks the (index, e2, e3, e5) state space through this repo's own Memoizer:
    // each nums[i] is multiplied in, divided out, or skipped.
    private static long CountSequencesByMemoSearch(int[] nums, (int E2, int E3, int E5) target)
    {
        var exponents = new (int E2, int E3, int E5)[nums.Length];
        for (var i = 0; i < nums.Length; i++)
        {
            exponents[i] = PrimeExponents(nums[i]);
        }

        return Memoizer.Memoize<(int Index, int E2, int E3, int E5), long>(
            (0, 0, 0, 0), new CountSequencesFromState(target, exponents));
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

    // One memoized step, named: at the end of nums the accumulated exponents are the
    // whole success test, otherwise all three choices branch off the same state.
    private sealed class CountSequencesFromState(
        (int E2, int E3, int E5) target,
        (int E2, int E3, int E5)[] exponents)
        : IRecurrence<(int Index, int E2, int E3, int E5), long>
    {
        public long Replay(
            (int Index, int E2, int E3, int E5) state,
            IRecurrence<(int Index, int E2, int E3, int E5), long> rest)
        {
            var (index, e2, e3, e5) = state;

            if (index == exponents.Length)
            {
                if (IsTargetReached((e2, e3, e5), target))
                {
                    return 1;
                }

                return 0;
            }

            var (d2, d3, d5) = exponents[index];

            return rest.Replay((index + 1, e2 + d2, e3 + d3, e5 + d5), rest)
                + rest.Replay((index + 1, e2 - d2, e3 - d3, e5 - d5), rest)
                + rest.Replay((index + 1, e2, e3, e5), rest);
        }
    }

    // The exponents accumulated so far are the target's - the whole success test,
    // since k's other prime factors were ruled out before the search started.
    private static bool IsTargetReached((int E2, int E3, int E5) current, (int E2, int E3, int E5) target)
        => current.E2 == target.E2 && current.E3 == target.E3 && current.E5 == target.E5;

    private static long BruteForceSearch(
        int[] nums, int index, (long Numerator, long Denominator) rational, long k)
    {
        if (index == nums.Length)
        {
            var valEqualsK = rational.Numerator % rational.Denominator == 0
                && rational.Numerator / rational.Denominator == k;
            return valEqualsK ? 1 : 0;
        }

        var value = nums[index];

        return BruteForceSearch(nums, index + 1, (rational.Numerator * value, rational.Denominator), k)
            + BruteForceSearch(nums, index + 1, (rational.Numerator, rational.Denominator * value), k)
            + BruteForceSearch(nums, index + 1, rational, k);
    }
}
