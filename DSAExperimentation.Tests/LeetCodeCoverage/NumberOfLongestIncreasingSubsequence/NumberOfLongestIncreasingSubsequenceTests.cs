using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfLongestIncreasingSubsequence;

// LeetCode 673. Number of Longest Increasing Subsequence: coordinate-compress nums
// into a rank per distinct value (this repo's own BinarySearch.LowerBound over an
// ArraySequence<int> of sorted distinct values - the same LowerBound engine
// LongestIncreasingSubsequenceTests already exercises), then sweep left to right
// maintaining a SegmentTree<(Length,Count),LisAggregate> keyed by rank. Query(0,
// rank-1) gives the best (longest length ending strictly before this value, and how
// many subsequences reach that length) among every smaller value seen so far;
// Update folds the new candidate into whatever's already stored for this exact
// value, so duplicate values never use each other as predecessors but still
// contribute to that value's own best-so-far. O(n log n) instead of the textbook
// O(n^2) double loop (see NumberOfLongestIncreasingSubsequenceBenchmarks for that
// comparison).
public sealed class NumberOfLongestIncreasingSubsequenceTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 5, 4, 7 }, 2)]
    [InlineData(new[] { 2, 2, 2, 2, 2 }, 5)]
    [InlineData(new[] { 1 }, 1)]
    [InlineData(new[] { 5, 4, 3, 2, 1 }, 5)]
    public void FindNumberOfLis_LeetCodeExamples_ReturnsExpectedCount(int[] nums, int expected)
        => Assert.Equal(expected, FindNumberOfLis(nums));

    private static int FindNumberOfLis(int[] nums)
    {
        var sortedDistinct = nums.Distinct().Order().ToArray();
        var ranks = new ArraySequence<int>(sortedDistinct);
        var tree = new SegmentTree<(int Length, int Count), LisAggregate>(new (int, int)[sortedDistinct.Length]);

        foreach (var num in nums)
        {
            var rank = BinarySearch.LowerBound<int, ArraySequence<int>>(ranks, num);
            var best = rank == 0 ? (Length: 0, Count: 0) : tree.Query(0, rank - 1);
            var candidate = best.Length == 0 ? (Length: 1, Count: 1) : (Length: best.Length + 1, best.Count);
            var existing = tree.Query(rank, rank);
            var combined = LisAggregate.Combine(existing, candidate);

            tree.Update(rank, combined);
        }

        return tree.Query(0, sortedDistinct.Length - 1).Count;
    }

    // Identity is (0,0): "no candidate yet" must combine away to whatever real value
    // it's paired with, the same MaxOperation<Element>.Identity = MinValue shape.
    // Combine prefers the longer length outright; on a tie it sums counts - the
    // standard "argmax with tie-count" aggregate, associative/commutative the same
    // way MaxOperation's plain max already is, since the result only ever depends on
    // the overall max length among everything combined and the summed count of
    // whichever elements attain it, never on grouping order.
    private readonly struct LisAggregate : ICombineOperation<(int Length, int Count)>
    {
        public static (int Length, int Count) Identity => (0, 0);

        public static (int Length, int Count) Combine((int Length, int Count) left, (int Length, int Count) right)
        {
            if (left.Length != right.Length)
            {
                return left.Length > right.Length ? left : right;
            }

            return (left.Length, left.Count + right.Count);
        }
    }
}
