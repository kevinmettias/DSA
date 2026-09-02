using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfSquareFreeSubsets;

// LeetCode 2572. Count the Number of Square-Free Subsets: nums[i] is in [1, 30], and
// a subset is square-free when its product has no repeated prime factor. Every value
// in [1, 30] factors over the same 10 primes <= 30, so "which primes divide v" is a
// 10-bit mask, precomputed once per value (-1 sentinel when v itself already has a
// squared prime factor, since including v then can never be square-free regardless of
// what else joins it). Counting then becomes: pick a subset of the distinct
// square-free values 2..30 whose masks are pairwise disjoint (each contributes its own
// multiplicity as a count, since picking "the same value" more than once would repeat
// its prime factors), times 2^(count of 1s) for the free choice each occurrence of 1
// contributes, minus 1 for the empty subset. Both strategies answer the same question
// with the same signature (TwoSumSolution precedent) so the test harness can assert
// agreement and the benchmark harness can time them against each other.
internal static class CountTheNumberOfSquareFreeSubsetsSolution
{
    private const int MaxValue = 30;
    private static readonly int[] Primes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29];
    private static readonly int[] PrimeMask = BuildPrimeMasks();

    // Textbook baseline: enumerates every one of the 2^n subsets directly, tracking the
    // running prime mask and bailing out the moment two chosen values share a prime -
    // the O(2^n * n) arm the bitmask DP has to beat.
    public static long CountByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var count = 0L;

        for (var subset = 1; subset < (1 << n); subset++)
        {
            if (IsSquareFreeSubset(nums, subset))
            {
                count++;
            }
        }

        return count % ModularArithmetic.Modulo;
    }

    private static bool IsSquareFreeSubset(int[] nums, int subset)
    {
        var mask = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((subset & (1 << i)) == 0)
            {
                continue;
            }

            var primeMask = PrimeMask[nums[i]];

            if (primeMask < 0 || (mask & primeMask) != 0)
            {
                return false;
            }

            mask |= primeMask;
        }

        return true;
    }

    // Groups nums by value first (order inside a subset never matters for the
    // product), then Memoizer walks the distinct square-free values 2..30 once,
    // deciding include/exclude per value and threading the accumulated prime mask
    // through as memoized state - a value already excluded whenever it collides with
    // the mask so far. Every occurrence of 1 is folded in afterward as a free
    // multiplier (2^ones), since 1 never changes squarefree-ness either way.
    public static long CountByBitmaskMemo(int[] nums)
    {
        var frequency = new int[MaxValue + 1];

        foreach (var num in nums)
        {
            frequency[num]++;
        }

        var squareFreeValues = new List<int>();

        for (var value = 2; value <= MaxValue; value++)
        {
            if (frequency[value] > 0 && PrimeMask[value] >= 0)
            {
                squareFreeValues.Add(value);
            }
        }

        var totalWithEmpty = Memoizer.Memoize<(int Index, int Mask), long>(
            (0, 0),
            (state, recurse) => CountFrom(state.Index, state.Mask, squareFreeValues, frequency, recurse));

        var multiplier = ModularArithmetic.Power(2, frequency[1]);
        var withOnes = totalWithEmpty * multiplier % ModularArithmetic.Modulo;

        return (withOnes - 1 + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    private static long CountFrom(
        int index, int mask, List<int> squareFreeValues, int[] frequency, Func<(int Index, int Mask), long> recurse)
    {
        if (index == squareFreeValues.Count)
        {
            return 1L;
        }

        var skip = recurse((index + 1, mask));
        var value = squareFreeValues[index];
        var primeMask = PrimeMask[value];

        if ((mask & primeMask) != 0)
        {
            return skip;
        }

        var take = frequency[value] * recurse((index + 1, mask | primeMask)) % ModularArithmetic.Modulo;
        return (skip + take) % ModularArithmetic.Modulo;
    }

    private static int[] BuildPrimeMasks()
    {
        var masks = new int[MaxValue + 1];

        for (var value = 1; value <= MaxValue; value++)
        {
            masks[value] = PrimeMaskFor(value);
        }

        return masks;
    }

    private static int PrimeMaskFor(int value)
    {
        var remaining = value;
        var mask = 0;

        for (var p = 0; p < Primes.Length && remaining > 1; p++)
        {
            if (remaining % Primes[p] != 0)
            {
                continue;
            }

            var exponent = 0;

            while (remaining % Primes[p] == 0)
            {
                remaining /= Primes[p];
                exponent++;
            }

            if (exponent > 1)
            {
                return -1;
            }

            mask |= 1 << p;
        }

        return mask;
    }
}
