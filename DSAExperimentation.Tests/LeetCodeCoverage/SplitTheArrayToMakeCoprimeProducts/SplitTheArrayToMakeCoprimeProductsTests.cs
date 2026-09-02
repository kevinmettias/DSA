using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitTheArrayToMakeCoprimeProducts;

// LeetCode 2584. Split the Array to Make Coprime Products: prime-factorize every
// value once into a HashMap<prime,lastIndex> recording each prime's rightmost
// occurrence - the same "map a key to the latest index that touches it" shape
// LargestComponentSizeByCommonFactorTests already establishes for shared prime
// factors, just tracking an index instead of a component owner. A left/right split
// at i is coprime exactly when no prime appearing in nums[0..i] ever reappears past
// i, so a single left-to-right sweep keeping a running "boundary" (the max
// last-occurrence among primes seen so far) finds the answer the moment
// boundary == i - no bignum products are ever computed.
public sealed class SplitTheArrayToMakeCoprimeProductsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 3, 3], 0 },
            { [4, 6, 8], -1 },
            { [2, 7], 0 },
            { [6, 2, 3, 35, 11], 2 },
            { [1, 1, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindValidSplit_LeetCodeStyleExamples_ReturnsSmallestCoprimeSplitIndex(int[] nums, int expected)
    {
        Assert.Equal(expected, FindValidSplit(nums));
    }

    private static int FindValidSplit(int[] nums)
    {
        var lastOccurrence = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.Set(factor, i);
            }
        }

        var boundary = 0;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.TryGetValue(factor, out var last);
                boundary = Math.Max(boundary, last);
            }

            if (boundary == i)
            {
                return i;
            }
        }

        return -1;
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = 2; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
