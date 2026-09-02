using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfGoodSubsets;

// LeetCode 1994. The Number of Good Subsets: bitmask DP over the 10 primes <= 30,
// expressed as a memoized recursion over (candidateIndex, usedPrimeMask) via this
// repo's own Memoizer - the same tuple-state shape NumberOfWaysToWearDifferentHats
// ToEachOtherTests.cs already proves for bitmask DP. Only squarefree values in
// [2, 30] can ever appear in a good subset (any repeated prime factor - e.g. 4 = 2^2 -
// makes every subset containing that value automatically bad), so those ~18 values are
// precomputed once as (PrimeMask, Weight) pairs, weight being how many times that value
// occurs in nums. f(index, usedMask) sums, over every squarefree candidate from index
// onward, the ways to either skip it or take it (weighted by its occurrence count) when
// its mask doesn't already overlap usedMask, bottoming out at 1 whenever the accumulated
// mask is non-empty (any non-empty mask is itself a valid good subset) and 0 otherwise.
// The count of 1s in nums never touches divisibility, so each occurrence independently
// doubles the final answer instead of ever entering the recursion.
public sealed partial class TheNumberOfGoodSubsetsTests
{
    private const int Modulo = 1_000_000_007;
    private static readonly int[] Primes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29];

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, 6)]
    [InlineData(new[] { 4, 2, 3, 15 }, 5)]
    public void NumberOfGoodSubsets_LeetCodeExamples_MatchesExpectedCount(int[] nums, int expected)
        => Assert.Equal(expected, NumberOfGoodSubsets(nums));

    private static int NumberOfGoodSubsets(int[] nums)
    {
        var counts = BuildCounts(nums);
        var candidates = BuildSquarefreeCandidates(counts);
        var waysWithNonEmptyMask = CountWaysWithNonEmptyMask(candidates);
        var doublingForOnes = Power(2, counts[1], Modulo);

        return (int)((waysWithNonEmptyMask * doublingForOnes) % Modulo);
    }

    private static int[] BuildCounts(int[] nums)
    {
        var counts = new int[31];

        foreach (var num in nums)
        {
            counts[num]++;
        }

        return counts;
    }

    private static long CountWaysWithNonEmptyMask(List<(int Mask, int Weight)> candidates)
    {
        return Memoizer.Memoize<(int Index, int UsedMask), long>((0, 0), (state, waysFor) =>
        {
            var (index, usedMask) = state;

            if (index == candidates.Count)
            {
                return usedMask != 0 ? 1L : 0L;
            }

            var (mask, weight) = candidates[index];
            var skip = waysFor((index + 1, usedMask));

            if ((usedMask & mask) != 0 || weight == 0)
            {
                return skip;
            }

            var take = (weight * waysFor((index + 1, usedMask | mask))) % Modulo;
            return (skip + take) % Modulo;
        });
    }

    private static List<(int Mask, int Weight)> BuildSquarefreeCandidates(int[] counts)
    {
        var candidates = new List<(int Mask, int Weight)>();

        for (var value = 2; value <= 30; value++)
        {
            if (TryComputeSquarefreePrimeMask(value, out var mask))
            {
                candidates.Add((mask, counts[value]));
            }
        }

        return candidates;
    }

    private static bool TryComputeSquarefreePrimeMask(int value, out int mask)
    {
        mask = 0;

        for (var i = 0; i < Primes.Length; i++)
        {
            var prime = Primes[i];
            if (value % prime != 0)
            {
                continue;
            }

            value /= prime;
            if (value % prime == 0)
            {
                mask = 0;
                return false;
            }

            mask |= 1 << i;
        }

        return true;
    }

    private static long Power(long value, int exponent, int modulo)
    {
        long result = 1;
        value %= modulo;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = (result * value) % modulo;
            }

            value = (value * value) % modulo;
            exponent >>= 1;
        }

        return result;
    }
}
