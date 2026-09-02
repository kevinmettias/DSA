using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioII;

// LeetCode 4013. Count Subarrays With Even Odd Ratio II: the exact same rule
// as LC 4011 (y > 0 and x*b <= y*a for a subarray's even count x and odd
// count y), but n up to 1e5 and nums[i]/a/b up to 1e9 rule out the O(n^2)
// enumeration - a count this large also needs a 64-bit accumulator, so the
// brute force here is kept only as a small-input correctness witness, not a
// benchmark contender at LeetCode's own scale.
//
// a*y - b*x >= 0 rearranges into a subarray-sum sign question: score every
// odd element +a and every even element -b, and count index pairs (L, R)
// with prefix[L] <= prefix[R]. Coordinate-compress the prefix sums via
// BinarySearch.LowerBound, then sweep left to right through this repo's own
// FenwickTree<int, SumOperation<int>> (a Binary Indexed Tree) - the same
// shape CountOfRangeSumTests already uses for LC 327, and CountSubarrays
// WithEvenOddRatioISolution proves out at the smaller LC 4011 scale first.
internal static class CountSubarraysWithEvenOddRatioIISolution
{
    // Every subarray scanned directly, extending y one element at a time -
    // O(n^2), BCL only. Only fast enough for LC 4011-sized inputs; kept here
    // purely as a correctness witness for the Fenwick sweep below.
    public static long CountByBruteForce(int[] nums, int a, int b)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var odd = 0;

            for (var right = left; right < nums.Length; right++)
            {
                odd += nums[right] % 2;
                var even = right - left + 1 - odd;

                if (odd > 0 && (long)even * b <= (long)odd * a)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // O(n log n): build the +a/-b weighted prefix sums (long throughout - a
    // single term can reach 1e9 and n reaches 1e5, so the running sum can
    // reach 1e14), coordinate-compress them, then for each prefix[R] query
    // the Fenwick tree for how many earlier prefixes already inserted are
    // <= prefix[R] before inserting prefix[R] itself.
    public static long CountByFenwickPrefixSweep(int[] nums, int a, int b)
    {
        var prefix = WeightedPrefixSums(nums, a, b);
        var sortedDistinct = prefix.Distinct().Order().ToArray();
        var ranks = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);

        var count = 0L;

        foreach (var prefixSum in prefix)
        {
            var rank = BinarySearch.LowerBound(ranks, prefixSum);
            count += tree.PrefixQuery(rank);
            tree.Add(rank, 1);
        }

        return count;
    }

    private static long[] WeightedPrefixSums(int[] nums, int a, int b)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + (nums[i] % 2 != 0 ? a : -b);
        }

        return prefix;
    }
}
