using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumSizeSubarraySum;

// LeetCode 209. Minimum Size Subarray Sum: nums are all positive, so the running
// prefix-sum array is strictly increasing - already "sorted ascending" in exactly
// the shape this repo's own BinarySearch.LowerBound assumes. For each start index
// i, LowerBound over an ArraySequence<int> witness finds the smallest end index
// whose cumulative sum first reaches target - an O(n log n) alternative to the
// textbook O(n) two-pointer sliding window, composing two existing production
// primitives (BinarySearch.LowerBound, ArraySequence<int>) instead of a hand-rolled
// scan, the same search-on-a-derived-monotonic-sequence idiom
// MedianOfTwoSortedArraysTests already established.
public sealed partial class MinimumSizeSubarraySumTests
{
    [Fact]
    public void MinSubArrayLen_ClassicExample_ReturnsShortestWindowLength()
        => Assert.Equal(2, MinSubArrayLen(7, [2, 3, 1, 2, 4, 3]));

    [Fact]
    public void MinSubArrayLen_ExactSingleElementMatch_ReturnsOne()
        => Assert.Equal(1, MinSubArrayLen(4, [1, 4, 4]));

    [Fact]
    public void MinSubArrayLen_TargetExceedsTotalSum_ReturnsZero()
        => Assert.Equal(0, MinSubArrayLen(11, [1, 1, 1, 1, 1, 1, 1, 1]));

    private static int MinSubArrayLen(int target, int[] nums)
    {
        var prefix = new int[nums.Length + 1];
        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        var sequence = new ArraySequence<int>(prefix);
        var best = int.MaxValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var end = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, target + prefix[i]);

            if (end <= nums.Length)
            {
                best = Math.Min(best, end - i);
            }
        }

        return best == int.MaxValue ? 0 : best;
    }
}
