using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumXorSumOfTwoArrays;

// LeetCode 1879. Minimum XOR Sum of Two Arrays: the textbook "assign each nums1[i]
// to a distinct nums2[j]" bitmask DP, expressed as a memoized recursion over the
// tuple state (index, mask) via this repo's own Memoizer - the identical (int, int)
// tuple-state shape MinimumCostToConnectTwoGroupsOfPointsTests already uses for LC
// 1595's near-identical "index, connected-mask" assignment recurrence, just XOR-ing
// instead of summing edge cost, and with no unmatched-point fallback needed since
// both arrays are the same length (a full permutation, not a partial matching).
public sealed partial class MinimumXorSumOfTwoArraysTests
{
    [Fact]
    public void MinimumXorSum_TwoElementExample_ReturnsMinimalTotal()
    {
        int[] nums1 = [1, 2];
        int[] nums2 = [2, 3];

        var actual = MinimumXorSum(nums1, nums2);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void MinimumXorSum_ThreeElementExample_ReturnsMinimalTotal()
    {
        int[] nums1 = [1, 0, 3];
        int[] nums2 = [5, 3, 4];

        var actual = MinimumXorSum(nums1, nums2);
        Assert.Equal(8, actual);
    }

    private static int MinimumXorSum(int[] nums1, int[] nums2)
    {
        var n = nums1.Length;

        return Memoizer.Memoize<(int Index, int Mask), int>((0, 0), (state, costFor) =>
        {
            var (index, mask) = state;

            if (index == n)
            {
                return 0;
            }

            var best = int.MaxValue;
            for (var j = 0; j < n; j++)
            {
                if ((mask & (1 << j)) != 0)
                {
                    continue;
                }

                var candidate = (nums1[index] ^ nums2[j]) + costFor((index + 1, mask | (1 << j)));
                best = Math.Min(best, candidate);
            }

            return best;
        });
    }
}
