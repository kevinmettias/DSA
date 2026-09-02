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
    {
        var actual = MinSwap(nums1, nums2);
        Assert.Equal(expected, actual);
    }

    // Precondition (guaranteed by LeetCode 801's own constraints): at least one valid
    // keep/swap assignment exists, so Cost below always finds a valid transition and
    // never has to reason about an all-invalid state.
    private static int MinSwap(int[] nums1, int[] nums2)
    {
        var arrays = new SwapArrays(nums1, nums2);
        var last = nums1.Length - 1;

        var keepCost = Memoizer.Memoize<(int Index, bool Swapped), int>((last, false), (state, cost) => Cost(arrays, state, cost));
        var swapCost = Memoizer.Memoize<(int Index, bool Swapped), int>((last, true), (state, cost) => Cost(arrays, state, cost));

        return Math.Min(keepCost, swapCost);
    }

    private static int Cost(SwapArrays arrays, (int Index, bool Swapped) state, Func<(int Index, bool Swapped), int> cost)
    {
        var (i, swapped) = state;

        if (i == 0)
        {
            return swapped ? 1 : 0;
        }

        var curA = swapped ? arrays.Nums2[i] : arrays.Nums1[i];
        var curB = swapped ? arrays.Nums1[i] : arrays.Nums2[i];
        var best = BestSwapChoice(arrays, i, (curA, curB), cost);

        return best + (swapped ? 1 : 0);
    }

    private static int BestSwapChoice(SwapArrays arrays, int i, (int CurA, int CurB) current, Func<(int Index, bool Swapped), int> cost)
    {
        var best = int.MaxValue;

        if (current.CurA > arrays.Nums1[i - 1] && current.CurB > arrays.Nums2[i - 1])
        {
            best = Math.Min(best, cost((i - 1, false)));
        }

        if (current.CurA > arrays.Nums2[i - 1] && current.CurB > arrays.Nums1[i - 1])
        {
            best = Math.Min(best, cost((i - 1, true)));
        }

        return best;
    }

    private readonly record struct SwapArrays(int[] Nums1, int[] Nums2);
}
