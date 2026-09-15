using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementI;

// LeetCode 3737. Count Subarrays With Majority Element I: count contiguous
// subarrays of nums whose majority element (the value occurring strictly more
// than half the subarray's length) equals target.
//
// Map every element to +1 (equals target) or -1 (does not); target is a
// subarray's majority element exactly when that mapped subarray sums to a
// positive value. With prefix[0..n] the running sum, subarray (i, j] sums
// positive iff prefix[j] > prefix[i] - so the whole problem reduces to counting
// index pairs i < j whose prefix sums are strictly increasing, the same
// coordinate-compression-plus-Fenwick-sweep shape
// CountOfRangeSumTests/CountOfSmallerNumbersAfterSelfTests already use for LC
// 327/315, just one-sided instead of two-sided.
internal static class CountSubarraysWithMajorityElementISolution
{
    // Textbook O(n^2): fix the start, extend the end one element at a time, and
    // track how many of the extended window's elements equal target directly -
    // the arm the Fenwick prefix-sum sweep below has to beat.
    public static int CountByBruteForce(int[] nums, int target)
    {
        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var targetCount = 0;

            for (var end = start; end < nums.Length; end++)
            {
                if (nums[end] == target)
                {
                    targetCount++;
                }

                if (targetCount * 2 > end - start + 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Composed: build prefix[0..n] from the +1/-1 mapping, coordinate-compress it
    // via BinarySearch.LowerBound over its sorted distinct values, then sweep
    // left to right through a FenwickTree<int, SumOperation<int>> (this repo's own
    // Binary Indexed Tree), counting - before inserting prefix[j] - how many
    // earlier prefix values were strictly smaller. Summing that count across
    // every j is exactly the number of majority subarrays.
    public static int CountByFenwickPrefixSum(int[] nums, int target)
    {
        var prefix = BuildPrefixSums(nums, target);

        var sortedDistinct = prefix.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var prefixSum in prefix)
        {
            var rank = BinarySearch.LowerBound(sequence, prefixSum);
            count += rank == 0 ? 0 : tree.PrefixQuery(rank - 1);
            tree.Add(rank, 1);
        }

        return count;
    }

    private static long[] BuildPrefixSums(int[] nums, int target)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            var isTarget = nums[i] == target;
            prefix[i + 1] = prefix[i] + (isTarget ? 1 : -1);
        }

        return prefix;
    }
}
