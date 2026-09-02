using ActiveFlipsQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfKConsecutiveBitFlips;

// LeetCode 995. Minimum Number of K Consecutive Bit Flips: a greedy left-to-right
// sweep tracking which flips are still "active" (started within the last k
// positions) via a FIFO of flip start indices - this repo's own Queue<TElement>,
// the same primitive ZeroOneMatrixTests uses as a BFS frontier, repurposed here as
// an expiring window instead of a traversal queue. queue.Count % 2 gives the
// current bit's cumulative flip parity in O(1), avoiding the textbook approach that
// actually mutates a k-length window on every flip.
public sealed partial class MinimumNumberOfKConsecutiveBitFlipsTests
{
    [Fact]
    public void MinKBitFlips_KEqualsOne_FlipsEachZeroIndividually()
    {
        int[] nums = [0, 1, 0];

        var actual = MinKBitFlips(nums, k: 1);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void MinKBitFlips_TrailingZeroCannotBeCovered_ReturnsNegativeOne()
    {
        int[] nums = [1, 1, 0];

        var actual = MinKBitFlips(nums, k: 2);
        Assert.Equal(-1, actual);
    }

    [Fact]
    public void MinKBitFlips_OverlappingFlipsRequired_ReturnsMinimumCount()
    {
        int[] nums = [0, 0, 0, 1, 0, 1, 1, 0];

        var actual = MinKBitFlips(nums, k: 3);
        Assert.Equal(3, actual);
    }

    private static int MinKBitFlips(int[] nums, int k)
    {
        var activeFlips = new ActiveFlipsQueue();
        var flipCount = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            ExpireFlip(activeFlips, k, i);

            var flipped = TryApplyFlip(activeFlips, nums, k, i);
            if (flipped is null)
            {
                return -1;
            }

            flipCount += flipped.Value;
        }

        return flipCount;
    }

    private static void ExpireFlip(ActiveFlipsQueue activeFlips, int k, int i)
    {
        if (activeFlips.TryPeek(out var earliestStart) && earliestStart + k == i)
        {
            activeFlips.TryDequeue(out _);
        }
    }

    private static int? TryApplyFlip(ActiveFlipsQueue activeFlips, int[] nums, int k, int i)
    {
        var effectiveBit = nums[i] ^ (activeFlips.Count % 2);

        if (effectiveBit != 0)
        {
            return 0;
        }

        if (i + k > nums.Length)
        {
            return null;
        }

        activeFlips.Enqueue(i);
        return 1;
    }
}
