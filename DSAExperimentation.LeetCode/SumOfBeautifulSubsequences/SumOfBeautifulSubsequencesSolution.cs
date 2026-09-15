using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.SumOfBeautifulSubsequences;

// LeetCode 3671. Sum of Beautiful Subsequences: for every positive integer g, the
// "beauty" of g is g times the count of strictly increasing subsequences whose GCD
// is exactly g. Return the sum of every beauty value, modulo 1e9+7.
//
// The composed strategy never computes an explicit GCD. Sum_S gcd(S) decomposes via
// the standard divisor-sieve trick: for a divisor g, f(g) = the count of strictly
// increasing subsequences drawn only from multiples of g has a GCD that is itself
// some multiple of g. Processing g from largest to smallest and subtracting every
// larger multiple m's already-known EXACT count isolates exact(g) - f(g) can only
// overcount subsequences whose true GCD is a proper multiple of g, and those are
// exactly the ones already peeled off. f(g) itself is "count every non-empty
// strictly increasing subsequence" over the multiples-of-g elements in their
// original order, which is DataStructures.FenwickTree's classic role: dp[i] = 1 +
// (sum of dp[j] for earlier j with a strictly smaller value), and the Fenwick tree
// over coordinate-compressed values turns that sum into one prefix query -
// Algorithms.Searching.BinarySearch.LowerBound supplies each value's compressed
// rank, the same composition MinimumStabilityFactorOfArray's GcdOperation doc
// comment points to for "the Query-with-a-custom-operation extensibility point
// this type exists for", here applied to Fenwick instead of SegmentTree.
internal static class SumOfBeautifulSubsequencesSolution
{
    // The textbook baseline: walk every subset via bitmask, keep only the strictly
    // increasing ones, fold each into its GCD with a hand-rolled Euclid (what
    // you'd write without this repo - ARCHITECTURE.md 17.5), and bucket by exact
    // GCD directly - no divisor sieve, no Fenwick tree.
    public static int SumBeautyByBruteForce(int[] nums)
    {
        var countByGcd = new Dictionary<int, long>();

        for (var mask = 1; mask < (1 << nums.Length); mask++)
        {
            if (!TryGcdOfIncreasingSubsequence(nums, mask, out var gcd))
            {
                continue;
            }

            countByGcd.TryGetValue(gcd, out var count);
            countByGcd[gcd] = count + 1;
        }

        var answer = 0L;

        foreach (var (gcd, count) in countByGcd)
        {
            answer = (answer + (gcd % ModularArithmetic.Modulo) * (count % ModularArithmetic.Modulo)) % ModularArithmetic.Modulo;
        }

        return (int)answer;
    }

    private static bool TryGcdOfIncreasingSubsequence(int[] nums, int mask, out int gcd)
    {
        var lastValue = -1;
        gcd = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            if (nums[i] <= lastValue)
            {
                return false;
            }

            lastValue = nums[i];
            gcd = Gcd(gcd, nums[i]);
        }

        return true;
    }

    public static int SumBeautyByDivisorSieve(int[] nums)
    {
        var maxValue = 0;

        foreach (var value in nums)
        {
            maxValue = Math.Max(maxValue, value);
        }

        var exactCount = new long[maxValue + 1];
        var answer = 0L;

        for (var g = maxValue; g >= 1; g--)
        {
            var exact = ExactCountForDivisor(nums, g, exactCount);

            exactCount[g] = exact;
            answer = (answer + (g % ModularArithmetic.Modulo) * exact) % ModularArithmetic.Modulo;
        }

        return (int)answer;
    }

    // The exact count for divisor g: every strictly increasing subsequence drawn from
    // the multiples of g, minus the counts already recorded for the larger multiples of
    // g - which are exactly the subsequences whose own GCD is a proper multiple of g.
    // Zero when g divides nothing in nums, which also contributes nothing to the sum.
    private static long ExactCountForDivisor(int[] nums, int g, long[] exactCount)
    {
        var multiples = FilterMultiples(nums, g);

        if (multiples.Count == 0)
        {
            return 0;
        }

        var total = CountIncreasingSubsequences(multiples);
        var largerMultiples = 0L;

        for (var multiple = 2 * g; multiple < exactCount.Length; multiple += g)
        {
            largerMultiples += exactCount[multiple];
        }

        largerMultiples %= ModularArithmetic.Modulo;

        return ((total - largerMultiples) % ModularArithmetic.Modulo + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    private static List<int> FilterMultiples(int[] nums, int g)
    {
        var multiples = new List<int>();

        foreach (var value in nums)
        {
            if (value % g == 0)
            {
                multiples.Add(value);
            }
        }

        return multiples;
    }

    // dp[i] = 1 + sum of dp[j] over earlier elements with a strictly smaller value;
    // the Fenwick tree turns "sum of dp for smaller-valued earlier elements" into
    // one O(log n) prefix query instead of an O(n) rescan.
    private static long CountIncreasingSubsequences(List<int> values)
    {
        var distinct = DistinctValues(values);
        var ranks = new ArraySequence<int>(distinct);
        var fenwick = new FenwickTree<long, SumOperation<long>>(distinct.Length);
        var total = 0L;

        foreach (var value in values)
        {
            total = AccumulateValue(ranks, fenwick, value, total);
        }

        return total;
    }

    // The distinct values in ascending order - the coordinate-compressed ranks' own
    // values, so a value's index in this array is its rank.
    private static int[] DistinctValues(List<int> values)
    {
        var sortedValues = values.ToArray();
        Array.Sort(sortedValues);

        var distinctCount = 0;

        foreach (var value in sortedValues)
        {
            if (distinctCount == 0 || sortedValues[distinctCount - 1] != value)
            {
                sortedValues[distinctCount++] = value;
            }
        }

        return sortedValues[..distinctCount];
    }

    // One element's contribution: its dp is one plus the sum of dp over the earlier
    // elements whose value is strictly smaller, which the Fenwick tree answers with a
    // single prefix query.
    private static long AccumulateValue(
        ArraySequence<int> ranks,
        FenwickTree<long, SumOperation<long>> fenwick,
        int value,
        long total)
    {
        var rank = BinarySearch.LowerBound(ranks, value);
        var before = rank == 0 ? 0 : fenwick.Query(0, rank - 1);
        var dp = (before + 1) % ModularArithmetic.Modulo;

        fenwick.Add(rank, dp);

        return (total + dp) % ModularArithmetic.Modulo;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
