using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumSwapsToMakeSequencesIncreasing;

// LeetCode 801. Minimum Swaps To Make Sequences Increasing: at each index, either
// keep or swap nums1[i]/nums2[i] so both arrays end up strictly increasing, for the
// minimum total swaps. Two-state DP - state (index, wasSwapped) -> minimum swaps for
// that prefix - via this repo's own Memoizer, the same HouseRobberII/DecodeWays
// shape, not the textbook flat dp[i][0]/dp[i][1] table.
public sealed partial class MinimumSwapsToMakeSequencesIncreasingTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 5, 4 }, new[] { 1, 2, 3, 7 }, 1)]
    [InlineData(new[] { 0, 3, 5, 8, 9 }, new[] { 2, 1, 4, 6, 9 }, 1)]
    public void MinSwap_LeetCodeExamples_ReturnsMinimumSwapCount(int[] nums1, int[] nums2, int expected)
        => Assert.Equal(expected, MinSwap(nums1, nums2));

    // Precondition (guaranteed by LeetCode 801's own constraints): at least one valid
    // keep/swap assignment exists, so Cost below always finds a valid transition and
    // never has to reason about an all-invalid state.
    private static int MinSwap(int[] nums1, int[] nums2)
    {
        var last = nums1.Length - 1;

        return Math.Min(
            Memoizer.Memoize<(int Index, bool Swapped), int>((last, false), Cost),
            Memoizer.Memoize<(int Index, bool Swapped), int>((last, true), Cost));

        int Cost((int Index, bool Swapped) state, Func<(int Index, bool Swapped), int> cost)
        {
            var (i, swapped) = state;

            if (i == 0)
            {
                return swapped ? 1 : 0;
            }

            var curA = swapped ? nums2[i] : nums1[i];
            var curB = swapped ? nums1[i] : nums2[i];
            var best = int.MaxValue;

            if (curA > nums1[i - 1] && curB > nums2[i - 1])
            {
                best = Math.Min(best, cost((i - 1, false)));
            }

            if (curA > nums2[i - 1] && curB > nums1[i - 1])
            {
                best = Math.Min(best, cost((i - 1, true)));
            }

            return best + (swapped ? 1 : 0);
        }
    }
}
