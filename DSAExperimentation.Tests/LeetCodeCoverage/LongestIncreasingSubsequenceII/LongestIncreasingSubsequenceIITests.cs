using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingSubsequenceII;

// LeetCode 2407. Longest Increasing Subsequence II: dp[v] = the length of the best
// valid subsequence ending exactly at value v. For each num, the best predecessor is
// the max dp over the value window [num-k, num-1] - this repo's own
// SegmentTree<int,MaxOperation<int>> keyed directly by value (not by rank the way
// NumberOfLongestIncreasingSubsequenceTests coordinate-compresses: the window here is
// defined in value-space, not subsequence-index-space, so rank-compression would
// break it). Query/Update are O(log maxValue), so the whole sweep is O(n log
// maxValue) instead of the textbook O(n^2) DP (see LongestIncreasingSubsequenceIIBenchmarks
// for that comparison).
public sealed partial class LongestIncreasingSubsequenceIITests
{
    [Theory]
    [InlineData(new[] { 4, 2, 1, 4, 3, 4, 5, 8, 15 }, 3, 5)]
    [InlineData(new[] { 7, 4, 5, 1, 8, 12, 4, 7 }, 5, 4)]
    [InlineData(new[] { 1, 5 }, 1, 1)]
    public void LengthOfLis_LeetCodeExamples_ReturnsExpectedLength(int[] nums, int k, int expected)
        => Assert.Equal(expected, LengthOfLis(nums, k));

    private static int LengthOfLis(int[] nums, int k)
    {
        var maxValue = nums.Max();
        var tree = new SegmentTree<int, MaxOperation<int>>(new int[maxValue + 1]);
        var best = 0;

        foreach (var num in nums)
        {
            var lo = Math.Max(0, num - k);
            var predecessor = tree.Query(lo, num - 1);
            var length = predecessor + 1;

            tree.Update(num, Math.Max(tree.Query(num, num), length));
            best = Math.Max(best, length);
        }

        return best;
    }
}
