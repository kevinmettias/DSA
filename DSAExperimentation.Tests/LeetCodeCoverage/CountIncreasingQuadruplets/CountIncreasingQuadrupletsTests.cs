using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountIncreasingQuadruplets;

// LeetCode 2552. Count Increasing Quadruplets: i<j<k<l with nums[i]<nums[k]<nums[j]<nums[l]
// pivots on the middle "inversion" pair (j,k) with j<k, nums[j]>nums[k] - the answer is the
// sum, over every such pair, of (count of i<j with nums[i]<nums[k]) times (count of l>k
// with nums[l]>nums[j]). Scanning k right to left, a plain running counter tracks the first
// factor (its threshold nums[k] is fixed for the whole inner j-loop, so no data structure is
// needed there), and this repo's own FenwickTree<int,SumOperation<int>> - a Binary Indexed
// Tree of already-inserted suffix values - answers the second factor as an O(log n)
// range-count query as l-candidates stream in from the right. Same "Fenwick tree of counts
// swept alongside a value-rank query" shape NumberOfPairsSatisfyingInequalityTests and
// CountOfSmallerNumbersAfterSelfTests already use, here swept right-to-left instead of
// left-to-right because the query side (l>k) is a suffix, not a prefix.
public sealed partial class CountIncreasingQuadrupletsTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 2, 4, 5 }, 2L)]
    [InlineData(new[] { 1, 2, 3, 4 }, 0L)]
    public void CountQuadruplets_LeetCodeExamples_ReturnsExpectedCount(int[] nums, long expected)
    {
        var actual = CountQuadruplets(nums);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CountQuadruplets_StrictlyDecreasingPermutation_FindsNoValidQuadruplet()
    {
        int[] nums = [5, 4, 3, 2, 1];

        var actual = CountQuadruplets(nums);

        Assert.Equal(0L, actual);
    }

    private static long CountQuadruplets(int[] nums)
    {
        var n = nums.Length;
        var suffixGreaterCounts = new FenwickTree<int, SumOperation<int>>(n);
        var total = 0L;

        for (var k = n - 1; k >= 0; k--)
        {
            var leftSmallerCount = 0;

            for (var j = 0; j < k; j++)
            {
                if (nums[j] > nums[k])
                {
                    var rightGreaterCount = nums[j] == n ? 0 : suffixGreaterCounts.Query(nums[j], n - 1);
                    total += (long)leftSmallerCount * rightGreaterCount;
                }
                else
                {
                    leftSmallerCount++;
                }
            }

            suffixGreaterCounts.Add(nums[k] - 1, 1);
        }

        return total;
    }
}
