using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SortedGcdPairQueries;

// LC 3312's own witness, meaningless outside it: precomputes, for every value v
// from 0 to max(nums), how many of the C(n,2) pairs have gcd(nums[i], nums[j]) <= v.
// Counting by GCD VALUE instead of enumerating pairs is the whole trick - a sieve
// over divisor counts (how many numbers are divisible by d) turns into an exact
// gcd-equals-d count by subtracting off every multiple of d already accounted for,
// high to low. Once built, answering "the k-th smallest gcd" is one
// BinarySearch.LowerBound lookup over the resulting cumulative-count array, the
// same "reduce the whole structure once, then binary-search it" shape
// FindMaximumNonDecreasingArrayLength already uses.
internal sealed class GcdPairCountIndex
{
    // Index v holds the number of pairs whose gcd is <= v; index 0 is the sentinel
    // "no pairs have gcd <= 0" (gcd is always positive), never itself a query answer.
    private readonly long[] _cumulativePairCountUpTo;

    private GcdPairCountIndex(long[] cumulativePairCountUpTo) => _cumulativePairCountUpTo = cumulativePairCountUpTo;

    public static GcdPairCountIndex Build(int[] nums)
    {
        var maxValue = LargestValue(nums);

        var countOfValue = CountOccurrences(nums, maxValue);

        var countDivisibleBy = CountDivisibleByEachValue(countOfValue, maxValue);
        var countGcdExactly = CountGcdExactlyEachValue(countDivisibleBy, maxValue);

        var cumulative = ToCumulativeCounts(countGcdExactly);

        return new GcdPairCountIndex(cumulative);
    }

    private static int LargestValue(int[] nums)
    {
        var maxValue = 0;

        foreach (var num in nums)
        {
            maxValue = Math.Max(maxValue, num);
        }

        return maxValue;
    }

    private static int[] CountOccurrences(int[] nums, int maxValue)
    {
        var countOfValue = new int[maxValue + 1];

        foreach (var num in nums)
        {
            countOfValue[num]++;
        }

        return countOfValue;
    }

    // How many nums are divisible by d, for every d - a harmonic-series sieve
    // (sum of maxValue/d over every d is O(maxValue log maxValue)), not an O(n *
    // maxValue) scan of the array itself per candidate divisor.
    private static int[] CountDivisibleByEachValue(int[] countOfValue, int maxValue)
    {
        var countDivisibleBy = new int[maxValue + 1];

        for (var divisor = 1; divisor <= maxValue; divisor++)
        {
            for (var multiple = divisor; multiple <= maxValue; multiple += divisor)
            {
                countDivisibleBy[divisor] += countOfValue[multiple];
            }
        }

        return countDivisibleBy;
    }

    // C(countDivisibleBy[d], 2) counts every pair whose gcd is a multiple of d, not
    // exactly d. Walking d from maxValue down to 1 and subtracting off every
    // already-resolved multiple of d strips that down to pairs whose gcd is exactly
    // d - the same high-to-low subtraction NumberOfDifferentSubsequencesGCDs'
    // achievable-gcd walk performs, generalized from a Set-membership check to a
    // pair count.
    private static long[] CountGcdExactlyEachValue(int[] countDivisibleBy, int maxValue)
    {
        var countGcdExactly = new long[maxValue + 1];

        for (var divisor = maxValue; divisor >= 1; divisor--)
        {
            var divisibleByCount = countDivisibleBy[divisor];
            var pairsDivisibleByDivisor = (long)divisibleByCount * (divisibleByCount - 1) / 2;

            for (var multiple = 2 * divisor; multiple <= maxValue; multiple += divisor)
            {
                pairsDivisibleByDivisor -= countGcdExactly[multiple];
            }

            countGcdExactly[divisor] = pairsDivisibleByDivisor;
        }

        return countGcdExactly;
    }

    // Index v in the result holds sum(countGcdExactly[1..v]), so a query's
    // lower-bound lookup is one read rather than a range sum.
    private static long[] ToCumulativeCounts(long[] countGcdExactly)
    {
        var cumulative = new long[countGcdExactly.Length];

        for (var value = 1; value < countGcdExactly.Length; value++)
        {
            cumulative[value] = cumulative[value - 1] + countGcdExactly[value];
        }

        return cumulative;
    }

    // The smallest gcd value v for which more than k pairs have gcd <= v - LC's
    // 0-indexed "k-th smallest" restated as a lower-bound lookup for k+1.
    public int KthSmallestGcd(int k)
        => BinarySearch.LowerBound<long, ArraySequence<long>>(new ArraySequence<long>(_cumulativePairCountUpTo), k + 1);
}
