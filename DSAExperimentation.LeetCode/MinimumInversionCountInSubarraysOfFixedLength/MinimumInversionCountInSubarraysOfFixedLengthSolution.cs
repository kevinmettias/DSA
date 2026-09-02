using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumInversionCountInSubarraysOfFixedLength;

// LeetCode 3768. Minimum Inversion Count in Subarrays of Fixed Length: among
// every contiguous subarray of length k, return the fewest inversions any of
// them has (an inversion is a pair i < j inside the subarray with
// nums[i] > nums[j]). The answer can exceed int range for large k, so both
// arms return long, matching LeetCode's own signature.
//
// Sliding a fixed-size window across nums and maintaining "how many
// inversions does the current window have" incrementally is the same
// coordinate-compression-plus-Fenwick-sweep CountSubarraysWithMajorityElementIISolution
// already uses for a different inversion-shaped count: rank each value, keep
// a FenwickTree<int, SumOperation<int>> of which ranks are currently in the
// window, and let each insertion/removal report its own inversion
// contribution straight from the tree's prefix sums. The window then slides
// one element at a time - remove the leftmost, insert the new rightmost -
// each side O(log n), for O(n log n) overall.
internal static class MinimumInversionCountInSubarraysOfFixedLengthSolution
{
    // The textbook approach: count every window's inversions from scratch
    // with a nested pairwise scan. O(n * k^2) - the arm the sliding-window
    // Fenwick strategy below has to beat.
    public static long MinInversionCountByBruteForce(int[] nums, int k)
    {
        var minInversions = long.MaxValue;

        for (var start = 0; start + k <= nums.Length; start++)
        {
            minInversions = Math.Min(minInversions, CountWindowInversions(nums, start, k));
        }

        return minInversions;
    }

    private static long CountWindowInversions(int[] nums, int start, int k)
    {
        var inversions = 0L;

        for (var i = start; i < start + k; i++)
        {
            for (var j = i + 1; j < start + k; j++)
            {
                if (nums[i] > nums[j])
                {
                    inversions++;
                }
            }
        }

        return inversions;
    }

    public static long MinInversionCountBySlidingWindowFenwick(int[] nums, int k)
    {
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);

        var inversions = 0L;
        var windowCount = 0;

        for (var i = 0; i < k; i++)
        {
            inversions += Insert(tree, sequence, nums[i], windowCount++);
        }

        var minInversions = inversions;

        for (var end = k; end < nums.Length; end++)
        {
            inversions -= Remove(tree, sequence, nums[end - k]);
            windowCount--;
            inversions += Insert(tree, sequence, nums[end], windowCount++);
            minInversions = Math.Min(minInversions, inversions);
        }

        return minInversions;
    }

    // Newly entering the window at the right: every already-present value
    // greater than this one now forms an inversion with it (earlier
    // position, larger value).
    private static long Insert(
        FenwickTree<int, SumOperation<int>> tree, ArraySequence<int> sequence, int value, int windowCountBeforeInsert)
    {
        var rank = BinarySearch.LowerBound(sequence, value);
        var lessOrEqualCount = tree.PrefixQuery(rank);
        tree.Add(rank, 1);

        return windowCountBeforeInsert - lessOrEqualCount;
    }

    // Leaving the window at the left: it was the earliest position in the
    // window, so every remaining value smaller than it was already forming
    // an inversion with it - undo exactly that contribution.
    private static long Remove(FenwickTree<int, SumOperation<int>> tree, ArraySequence<int> sequence, int value)
    {
        var rank = BinarySearch.LowerBound(sequence, value);
        var lessCount = rank == 0 ? 0 : tree.PrefixQuery(rank - 1);
        tree.Add(rank, -1);

        return lessCount;
    }
}
