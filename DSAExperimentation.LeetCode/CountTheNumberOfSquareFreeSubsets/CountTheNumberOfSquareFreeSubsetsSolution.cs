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
        var frequency = TallyFrequencies(nums);
        var squareFreeValues = SquareFreeValuesIn(frequency);

        var totalWithEmpty = Memoizer.Memoize<(int Index, int Mask), long>(
            (0, 0),
            new SubsetsFromRemainingValues(squareFreeValues, frequency));

        var multiplier = ModularArithmetic.Power(2, frequency[1]);
        var withOnes = totalWithEmpty * multiplier % ModularArithmetic.Modulo;

        return (withOnes - 1 + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    // How many times each value in [0, MaxValue] occurs. Order inside a subset never
    // matters for the product, so the occurrences collapse to a multiplicity here.
    private static int[] TallyFrequencies(int[] nums)
    {
        var frequency = new int[MaxValue + 1];

        foreach (var num in nums)
        {
            frequency[num]++;
        }

        return frequency;
    }

    // The distinct values above 1 that could join a square-free subset at all: present
    // at least once, and not already carrying a squared prime factor of their own.
    private static List<int> SquareFreeValuesIn(int[] frequency)
    {
        var values = new List<int>();

        for (var value = 2; value <= MaxValue; value++)
        {
            if (frequency[value] > 0 && PrimeMask[value] >= 0)
            {
                values.Add(value);
            }
        }

        return values;
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
            var extracted = RemovePrimeFactor(remaining, mask, p);

            if (!extracted.SquareFree)
            {
                return -1;
            }

            (remaining, mask) = (extracted.Remaining, extracted.Mask);
        }

        return mask;
    }

    // One prime's share of the factorization of `remaining`: the mask gains that prime's
    // bit when it divides the value exactly once, gains nothing when it does not divide
    // it at all, and answers SquareFree: false when it divides it twice over - a squared
    // prime factor, which no subset containing that value can ever be square-free with.
    private static (int Remaining, int Mask, bool SquareFree) RemovePrimeFactor(
        int remaining, int mask, int primeIndex)
    {
        var (reduced, exponent) = DivideOut(remaining, Primes[primeIndex]);

        if (exponent == 0)
        {
            return (reduced, mask, true);
        }

        if (exponent == 1)
        {
            return (reduced, mask | (1 << primeIndex), true);
        }

        return (remaining, mask, false);
    }

    // How many times `prime` divides `value`, and what is left of `value` once every one
    // of those factors has been divided out.
    private static (int Reduced, int Exponent) DivideOut(int value, int prime)
    {
        var reduced = value;
        var exponent = 0;

        while (reduced % prime == 0)
        {
            reduced /= prime;
            exponent++;
        }

        return (reduced, exponent);
    }

    // The recurrence, as a named type: values are walked in one fixed order, and each
    // is either left out outright or taken - taking it only when its primes do not
    // collide with the mask so far, and contributing one choice per occurrence.
    private sealed class SubsetsFromRemainingValues(List<int> squareFreeValues, int[] frequency)
        : IRecurrence<(int Index, int Mask), long>
    {
        public long Replay((int Index, int Mask) state, IRecurrence<(int Index, int Mask), long> rest)
        {
            if (state.Index == squareFreeValues.Count)
            {
                return 1L;
            }

            var skip = rest.Replay((state.Index + 1, state.Mask), rest);
            var value = squareFreeValues[state.Index];
            var primeMask = PrimeMask[value];

            if ((state.Mask & primeMask) != 0)
            {
                return skip;
            }

            var take = frequency[value] * rest.Replay((state.Index + 1, state.Mask | primeMask), rest) % ModularArithmetic.Modulo;
            return (skip + take) % ModularArithmetic.Modulo;
        }
    }
}
