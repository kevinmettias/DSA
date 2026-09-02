using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitArrayLargestSum;

// LeetCode 410. Split Array Largest Sum: "binary search on the answer" - the
// feasibility of a candidate limit ("can nums be split into <= k contiguous
// subarrays each summing to at most limit?") is monotone non-decreasing in
// limit, so the minimum feasible limit is the leftmost "true" in an implicit
// [false...false, true...true] sequence. FeasibleSplitSequence computes that
// boolean on demand (GridChildren's "computed, not stored" precedent, applied
// to IRandomAccessSequence<bool> instead of IChildren) so this repo's own
// BinarySearch.LowerBound can locate it directly, instead of hand-rolling a
// second int lo/hi bisection loop.
public sealed partial class SplitArrayLargestSumTests
{
    [Theory]
    [InlineData(new[] { 7, 2, 5, 10, 8 }, 2, 18)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 9)]
    [InlineData(new[] { 1, 4, 4 }, 3, 4)]
    public void MinimizedLargestSum_LeetCodeExamples_ReturnsSmallestFeasibleMax(int[] nums, int k, int expected)
    {
        var actual = MinimizedLargestSum(nums, k);
        Assert.Equal(expected, actual);
    }

    private static int MinimizedLargestSum(int[] nums, int k)
    {
        var floor = nums.Max();
        var ceiling = nums.Sum();
        var sequence = new FeasibleSplitSequence(nums, k, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

    private static bool CanSplitWithinLimit(int[] nums, int k, int limit)
    {
        var subarrays = 1;
        var currentSum = 0;

        foreach (var num in nums)
        {
            if (currentSum + num > limit)
            {
                subarrays++;
                currentSum = 0;
            }

            currentSum += num;
        }

        return subarrays <= k;
    }

    private readonly struct FeasibleSplitSequence(int[] nums, int k, int floor, int ceiling) : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanSplitWithinLimit(nums, k, floor + index);
    }
}
