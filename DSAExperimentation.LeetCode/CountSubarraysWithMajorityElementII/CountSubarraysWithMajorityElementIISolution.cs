using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementII;

// LeetCode 3739. Count Subarrays With Majority Element II: the same question as
// LC 3737 ((CountSubarraysWithMajorityElementISolution's own doc comment) - count
// contiguous subarrays whose majority element equals target), at a scale (n up to
// 1e5) where I's O(n^2) brute force is too slow to be the intended solution, and
// where the answer itself can exceed int.MaxValue (an all-target array counts
// every one of its ~n^2/2 subarrays), so both arms here return long.
//
// Mapping nums[i] to +1/-1 (equals target or not) turns "target is the majority"
// into "the mapped subarray sums positive", and with prefix[0..n] the running
// sum, subarray (i, j] sums positive iff prefix[j] > prefix[i]. Coordinate-
// compressing prefix and sweeping it left to right through a FenwickTree<int,
// SumOperation<int>> counts, for each j, how many earlier prefix[i] are strictly
// smaller - the same one-sided coordinate-compression-plus-Fenwick-sweep
// CountOfSmallerNumbersAfterSelfTests already uses for LC 315, in O(n log n).
internal static class CountSubarraysWithMajorityElementIISolution
{
    // The textbook O(n^2) scan, unchanged in shape from
    // CountSubarraysWithMajorityElementISolution's own baseline - correct at any
    // size, just the arm the Fenwick prefix-sum sweep below has to beat once n
    // grows past what a quadratic scan can finish in time.
    public static long CountByBruteForce(int[] nums, int target)
    {
        var count = 0L;

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

    // The one implementation of the sweep the class doc derives: LC 3737's own
    // CountByFenwickPrefixSum calls this and narrows the total to its int answer.
    public static long CountByFenwickPrefixSum(int[] nums, int target)
    {
        var prefix = BuildPrefixSums(nums, target);

        var sortedDistinct = prefix.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0L;

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
