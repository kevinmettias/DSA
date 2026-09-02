using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithFixedBounds;

// LeetCode 2444. Count Subarrays With Fixed Bounds: a subarray's minimum equals
// minK AND its maximum equals maxK if and only if every one of its elements already
// lies within [minK, maxK] - min==minK forces every element >= minK, max==maxK
// forces every element <= maxK, so no separate range check is needed on top of the
// two equalities. Every subarray's min/max is answered in O(log n) by this repo's
// own SegmentTree<int,MinOperation<int>>/SegmentTree<int,MaxOperation<int>> pair -
// the same Min/Max SegmentTree composition BookingConcertTicketsInGroupsTests
// already uses, queried once per (start,end) pair instead of rescanning each
// subarray from scratch.
public sealed partial class CountSubarraysWithFixedBoundsTests
{
    [Fact]
    public void CountFixedBoundSubarrays_LeetCodeExampleOne_ReturnsTwoQualifyingSubarrays()
    {
        int[] nums = [1, 3, 5, 2, 7, 5];

        var count = CountFixedBoundSubarrays(nums, minK: 1, maxK: 5);

        Assert.Equal(2, count);
    }

    [Fact]
    public void CountFixedBoundSubarrays_LeetCodeExampleTwo_EveryElementEqualsBothBounds_CountsEverySubarray()
    {
        int[] nums = [1, 1, 1, 1];

        var count = CountFixedBoundSubarrays(nums, minK: 1, maxK: 1);

        Assert.Equal(10, count);
    }

    [Fact]
    public void CountFixedBoundSubarrays_NoElementReachesMaxK_ReturnsZero()
    {
        int[] nums = [2, 2, 2];

        var count = CountFixedBoundSubarrays(nums, minK: 1, maxK: 5);

        Assert.Equal(0, count);
    }

    private static long CountFixedBoundSubarrays(int[] nums, int minK, int maxK)
    {
        var minTree = new SegmentTree<int, MinOperation<int>>(nums);
        var maxTree = new SegmentTree<int, MaxOperation<int>>(nums);
        long count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            for (var end = start; end < nums.Length; end++)
            {
                if (minTree.Query(start, end) == minK && maxTree.Query(start, end) == maxK)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
