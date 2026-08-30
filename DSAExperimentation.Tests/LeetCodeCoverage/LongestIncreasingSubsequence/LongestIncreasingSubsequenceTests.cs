using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingSubsequence;

// LeetCode 300. Longest Increasing Subsequence: patience sorting - each number either
// extends the current "tails" run or overwrites the first tail it is not smaller
// than, found via this repo's own BinarySearch.LowerBound (same engine
// SearchInsertPositionTests already exercises) over a DynamicArraySequence view of a
// DynamicArray<int>, so Length tracks the run as it grows - giving O(n log n) instead
// of the textbook O(n^2) DP.
public sealed partial class LongestIncreasingSubsequenceTests
{
    [Theory]
    [InlineData(new[] { 10, 9, 2, 5, 3, 7, 101, 18 }, 4)]
    [InlineData(new[] { 0, 1, 0, 3, 2, 3 }, 4)]
    [InlineData(new[] { 7, 7, 7, 7, 7, 7, 7 }, 1)]
    public void LengthOfLis_LeetCodeExamples_ReturnsExpectedLength(int[] nums, int expected)
        => Assert.Equal(expected, LengthOfLis(nums));

    private static int LengthOfLis(int[] nums)
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
