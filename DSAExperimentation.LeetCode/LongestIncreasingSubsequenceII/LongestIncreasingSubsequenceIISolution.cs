using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.LongestIncreasingSubsequenceII;

// LeetCode 2407. Longest Increasing Subsequence II: the longest strictly increasing
// subsequence of nums in which consecutive picks differ by at most k, spelled
// `maxGap` here because the letter says nothing on its own.
//
// Both strategies compute the same dp - "the best valid subsequence length ending at
// this element" - and differ only in how they find the best predecessor. The
// textbook DP rescans every earlier element; the composed arm keeps the dp indexed
// by *value* and asks a range-max query for it.
internal static class LongestIncreasingSubsequenceIISolution
{
    // The textbook O(n^2) DP: for each element, rescan every earlier one and take the
    // best that satisfies both constraints - strictly smaller, and within maxGap.
    // BCL-only, the arm the composed solution below has to justify itself against.
    public static int LengthOfLisByDynamicProgramming(int[] nums, int maxGap)
    {
        var dp = new int[nums.Length];
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            dp[i] = BestLengthEndingAt(nums, dp, maxGap, i);
            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    private static int BestLengthEndingAt(int[] nums, int[] dp, int maxGap, int index)
    {
        var length = 1;

        for (var j = 0; j < index; j++)
        {
            // A valid predecessor is strictly smaller and no further away than maxGap
            // - the two halves of the constraint LC 2407 adds to a plain LIS.
            var isValidPredecessor = nums[j] < nums[index] && nums[index] - nums[j] <= maxGap;

            if (isValidPredecessor && dp[j] + 1 > length)
            {
                length = dp[j] + 1;
            }
        }

        return length;
    }

    // This repo's own SegmentTree<int, MaxOperation<int>> keyed directly by value:
    // slot v holds the best valid subsequence length ending exactly at value v, so
    // the best predecessor for num is a single range-max over [num - maxGap, num - 1]
    // - the value window the problem itself defines. Keying by value rather than by
    // coordinate-compressed rank is forced here: the window lives in value-space, so
    // the rank compression NumberOfLongestIncreasingSubsequenceSolution uses would
    // move its endpoints. O(n log maxValue) instead of the O(n^2) above.
    public static int LengthOfLisBySegmentTreeValueWindow(int[] nums, int maxGap)
    {
        var maxValue = nums.Max();
        var tree = new SegmentTree<int, MaxOperation<int>>(new int[maxValue + 1]);
        var best = 0;

        foreach (var num in nums)
        {
            var windowStart = Math.Max(0, num - maxGap);
            var predecessor = tree.Query(windowStart, num - 1);
            var length = predecessor + 1;
            var storedForThisValue = tree.Query(num, num);
            var bestForThisValue = Math.Max(storedForThisValue, length);

            tree.Update(num, bestForThisValue);
            best = Math.Max(best, length);
        }

        return best;
    }
}
