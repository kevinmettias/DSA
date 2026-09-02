using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

using RepoSegmentTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<long, DSAExperimentation.DataStructures.SegmentTree.MaxOperation<long>>;

namespace DSAExperimentation.LeetCode.MaximumBalancedSubsequenceSum;

// LeetCode 2926. Maximum Balanced Subsequence Sum: nums[i_j] - nums[i_j-1] >= i_j - i_j-1
// for every consecutive pair rearranges to (nums[i_j] - i_j) >= (nums[i_j-1] - i_j-1) - so
// with b[i] = nums[i] - i, a balanced subsequence is exactly one whose b-values are
// non-decreasing along increasing index. Maximize the sum of the original nums[i]
// values over such a subsequence.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MaximumBalancedSubsequenceSumSolution
{
    // Textbook O(n^2): dp[i] = nums[i] + max(0, best dp[j] for j < i with
    // b[j] <= b[i]) - never chain onto a negative-sum predecessor, since a
    // length-1 subsequence is always valid on its own. The arm the segment-tree
    // strategy has to beat.
    public static long MaxBalancedSumByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var b = new long[n];
        for (var i = 0; i < n; i++)
        {
            b[i] = (long)nums[i] - i;
        }

        var dp = new long[n];
        var answer = long.MinValue;

        for (var i = 0; i < n; i++)
        {
            var best = 0L;
            for (var j = 0; j < i; j++)
            {
                if (b[j] <= b[i])
                {
                    best = Math.Max(best, dp[j]);
                }
            }

            dp[i] = nums[i] + best;
            answer = Math.Max(answer, dp[i]);
        }

        return answer;
    }

    // Composed: coordinate-compress b[i] = nums[i] - i via this repo's own
    // BinarySearch.LowerBound over the sorted distinct b-values (same idiom
    // CountOfSmallerNumbersAfterSelfTests uses for a Fenwick-tree sweep), then
    // sweep left to right through a SegmentTree<long, MaxOperation<long>> holding,
    // per rank, the best subsequence sum achieved by any earlier index whose
    // b-value compresses to that rank. Query(0, rank) reads the best sum among
    // every b-value <= b[i] in one O(log n) call - a plain FenwickTree can't play
    // this role, since max has no group inverse for its own range-query trick to
    // subtract back out (see FenwickTree's own doc comment) - a segment tree needs
    // no inverse at all.
    public static long MaxBalancedSumBySegmentTree(int[] nums)
    {
        var n = nums.Length;
        var b = new long[n];
        for (var i = 0; i < n; i++)
        {
            b[i] = (long)nums[i] - i;
        }

        var sortedDistinct = b.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var initial = new long[sortedDistinct.Length];
        Array.Fill(initial, long.MinValue);
        var tree = new RepoSegmentTree(initial);

        var answer = long.MinValue;

        for (var i = 0; i < n; i++)
        {
            var rank = BinarySearch.LowerBound(sequence, b[i]);
            var bestBefore = tree.Query(0, rank);
            var dp = nums[i] + Math.Max(0, bestBefore);

            tree.Update(rank, Math.Max(tree.Query(rank, rank), dp));
            answer = Math.Max(answer, dp);
        }

        return answer;
    }
}
