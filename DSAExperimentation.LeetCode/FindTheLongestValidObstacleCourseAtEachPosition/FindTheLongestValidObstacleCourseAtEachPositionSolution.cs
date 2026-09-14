using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindTheLongestValidObstacleCourseAtEachPosition;

// LeetCode 1964. Find the Longest Valid Obstacle Course at Each Position: for every
// index, the length of the longest non-decreasing subsequence of obstacles that ends
// there. "Valid course" only requires non-decreasing height, not strictly increasing,
// so equal heights extend a run instead of ending it.
//
// The two strategies answer that with the classic quadratic DP and with patience
// sorting over a "tails" buffer.
internal static class FindTheLongestValidObstacleCourseAtEachPositionSolution
{
    // Baseline: dp[i] = 1 + max(dp[j]) over every earlier j whose obstacle is no taller,
    // found by rescanning the whole prefix. O(n^2), plain BCL arrays throughout.
    public static int[] LongestObstacleCourseByDynamicProgramming(int[] obstacles)
    {
        var dp = new int[obstacles.Length];

        for (var i = 0; i < obstacles.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (obstacles[j] <= obstacles[i] && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }
        }

        return dp;
    }

    // The same patience-sorting "tails" array LongestIncreasingSubsequence builds via
    // this repo's own BinarySearch over a DynamicArraySequence<int>, using UpperBound
    // rather than LowerBound so an equal height extends the run. The insertion position
    // doubles as that index's answer for free: it is exactly how many tails are already
    // no taller than this obstacle, i.e. the longest non-decreasing run ending here.
    // O(n log n).
    public static int[] LongestObstacleCourseByPatienceSorting(int[] obstacles)
    {
        var tails = new DynamicArray<int>();
        var answer = new int[obstacles.Length];

        for (var i = 0; i < obstacles.Length; i++)
        {
            var position = BinarySearch.UpperBound(new DynamicArraySequence<int>(tails), obstacles[i]);
            answer[i] = position + 1;

            if (position == tails.Count)
            {
                tails.Add(obstacles[i]);
            }
            else
            {
                tails.Set(position, obstacles[i]);
            }
        }

        return answer;
    }
}
