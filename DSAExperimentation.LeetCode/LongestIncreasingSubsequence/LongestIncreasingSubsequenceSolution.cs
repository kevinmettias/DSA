using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.LongestIncreasingSubsequence;

// LeetCode 300. Longest Increasing Subsequence: the length of the longest strictly
// increasing subsequence of nums.
//
// The textbook baseline is O(n^2) DP: dp[i] is the longest run ending at i, found by
// rescanning every earlier index. Patience sorting gets the same length in
// O(n log n) - each number either extends the current "tails" run or overwrites the
// first tail it is not smaller than, found via this repo's own BinarySearch.LowerBound
// (the same engine SearchInsertPositionTests already exercises) over a
// DynamicArraySequence view of a DynamicArray<int>, so Length tracks the run as it
// grows.
internal static class LongestIncreasingSubsequenceSolution
{
    // The textbook answer: BCL int[] DP table, deliberately written without this
    // repo's primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static int LengthOfLisByDynamicProgramming(int[] nums)
    {
        var dp = new int[nums.Length];
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (nums[j] < nums[i] && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }

            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    // Patience sorting: "tails" holds the smallest possible tail value for every run
    // length seen so far, so its Length is the answer once every number is placed.
    public static int LengthOfLisByPatienceSortingBinarySearch(int[] nums)
    {
        var tails = new DynamicArray<int>();

        foreach (var num in nums)
        {
            var position = BinarySearch.LowerBound(new DynamicArraySequence<int>(tails), num);

            if (position == tails.Count)
            {
                tails.Add(num);
            }
            else
            {
                tails.Set(position, num);
            }
        }

        return tails.Count;
    }
}
