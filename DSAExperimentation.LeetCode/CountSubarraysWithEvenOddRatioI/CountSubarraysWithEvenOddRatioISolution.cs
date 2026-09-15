using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioI;

// LeetCode 4011. Count Subarrays With Even Odd Ratio I: for a subarray with x
// even elements and y odd elements, it is valid when y > 0 and x/y <= a/b -
// compared via the equivalent cross-multiplied integer inequality x*b <= y*a
// so no floating point ever enters the comparison. n <= 1000 keeps the
// O(n^2) enumeration cheap enough to serve as the textbook baseline here.
//
// Rearranged the other way, a*y - b*x >= 0 is a subarray-sum sign question:
// score every odd element +a and every even element -b, and count index
// pairs (L, R) whose prefix sums satisfy prefix[L] <= prefix[R]. That is
// exactly the coordinate-compression-plus-Fenwick-sweep shape
// CountOfRangeSumTests already uses for LC 327, and it is the strategy LC
// 4013 (same rule, n up to 1e5) needs to stay subquadratic - this class
// proves it out at the smaller size first.
internal static class CountSubarraysWithEvenOddRatioISolution
{
    // Every subarray scanned directly, extending y one element at a time -
    // O(n^2), BCL only. The arm the Fenwick sweep below has to beat.
    public static int CountByBruteForce(int[] nums, int a, int b)
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

        return (int)count;
    }

    // One O(n log n) left-to-right sweep: build the +a/-b weighted prefix
    // sums, coordinate-compress them via BinarySearch.LowerBound over their
    // sorted distinct values, then for each prefix[R] query this repo's own
    // FenwickTree<int, SumOperation<int>> (a Binary Indexed Tree) for how
    // many earlier prefixes already inserted are <= prefix[R] before
    // inserting prefix[R] itself.
    public static int CountByFenwickPrefixSweep(int[] nums, int a, int b)
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

        return (int)count;
    }

    private static long[] WeightedPrefixSums(int[] nums, int a, int b)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            var isOdd = nums[i] % 2 != 0;
            prefix[i + 1] = prefix[i] + (isOdd ? a : -b);
        }

        return prefix;
    }
}
