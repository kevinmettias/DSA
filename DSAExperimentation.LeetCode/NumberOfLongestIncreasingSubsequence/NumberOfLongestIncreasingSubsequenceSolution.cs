using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.NumberOfLongestIncreasingSubsequence;

// LeetCode 673. Number of Longest Increasing Subsequence: how many distinct
// subsequences of nums attain the longest strictly-increasing length.
//
// FindNumberOfLisByBruteForce is the textbook O(n^2) DP this repo's own
// composition has to justify itself against: for each i, rescan every earlier j
// tracking both the longest length ending at i and how many subsequences reach
// it, written without this repo's primitives.
//
// FindNumberOfLisBySegmentTree coordinate-compresses nums into a rank per
// distinct value (this repo's own BinarySearch.LowerBound over an
// ArraySequence<int> of sorted distinct values - the same LowerBound engine
// LongestIncreasingSubsequenceTests already exercises), then sweeps left to
// right maintaining a SegmentTree<(Length,Count),LisAggregate> keyed by rank.
// Query(0, rank-1) gives the best (longest length ending strictly before this
// value, and how many subsequences reach that length) among every smaller value
// seen so far; Update folds the new candidate into whatever's already stored for
// this exact value, so duplicate values never use each other as predecessors but
// still contribute to that value's own best-so-far. O(n log n) instead of the
// O(n^2) above.
internal static class NumberOfLongestIncreasingSubsequenceSolution
{
    public static int FindNumberOfLisByBruteForce(int[] nums)
    {
        var length = new int[nums.Length];
        var count = new int[nums.Length];
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            var currentLength = ComputeLengthAndCount(nums, length, count, i);
            best = Math.Max(best, currentLength);
        }

        var total = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (length[i] == best)
            {
                total += count[i];
            }
        }

        return total;
    }

    private static int ComputeLengthAndCount(int[] nums, int[] length, int[] count, int index)
    {
        length[index] = 1;
        count[index] = 1;

        for (var j = 0; j < index; j++)
        {
            if (nums[j] >= nums[index])
            {
                continue;
            }

            if (length[j] + 1 > length[index])
            {
                length[index] = length[j] + 1;
                count[index] = count[j];
            }
            else if (length[j] + 1 == length[index])
            {
                count[index] += count[j];
            }
        }

        return length[index];
    }

    public static int FindNumberOfLisBySegmentTree(int[] nums)
    {
        var sortedDistinct = nums.Distinct().Order().ToArray();
        var ranks = new ArraySequence<int>(sortedDistinct);
        var tree = new SegmentTree<(int Length, int Count), LisAggregate>(new (int, int)[sortedDistinct.Length]);

        foreach (var num in nums)
        {
            var rank = BinarySearch.LowerBound<int, ArraySequence<int>>(ranks, num);
            var best = rank == 0 ? NoSubsequence() : tree.Query(0, rank - 1);
            var candidate = best.Length == 0 ? NewSubsequence() : ExtendedSubsequence(best);
            var existing = tree.Query(rank, rank);
            var combined = LisAggregate.Combine(existing, candidate);

            tree.Update(rank, combined);
        }

        return tree.Query(0, sortedDistinct.Length - 1).Count;
    }

    // The best-so-far aggregate when no smaller value has been seen yet: there is
    // nothing to extend, so no length and no count.
    private static (int Length, int Count) NoSubsequence() => (Length: 0, Count: 0);

    // The aggregate for a value that starts a subsequence of its own: length one,
    // reached one way.
    private static (int Length, int Count) NewSubsequence() => (Length: 1, Count: 1);

    // The aggregate for a value appended to the best-so-far subsequence: one
    // longer, with every one of that best's subsequences still reaching it.
    private static (int Length, int Count) ExtendedSubsequence((int Length, int Count) best) =>
        (Length: best.Length + 1, best.Count);
}
