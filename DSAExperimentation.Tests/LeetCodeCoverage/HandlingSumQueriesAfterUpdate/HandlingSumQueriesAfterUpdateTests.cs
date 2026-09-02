using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HandlingSumQueriesAfterUpdate;

// LeetCode 2569. Handling Sum Queries After Update: type-1 queries range-flip
// nums1's bits, type-2 queries add p times the CURRENT count of ones in nums1 to a
// running sum of nums2, and type-3 queries read that running sum back. Tracking
// "count of ones in a range, under range flips" is exactly this repo's own
// LazySegmentTree<Element,TUpdate,TOperation> generalized via a third
// IRangeUpdateOperation<int,bool> witness - a flip toggles a range's 1-count to
// (rangeLength - count) (ApplyUpdate), and two pending flips on the same node
// cancel out (ComposeUpdate is XOR) - the same "compose the generic lazy-propagation
// engine over a new algebra" move FancySequenceTests' AffineOperation and this
// repo's own RangeAddSumOperation/RangeAssignMaxOperation already make.
public sealed partial class HandlingSumQueriesAfterUpdateTests
{
    [Fact]
    public void HandleQuery_LeetCodeExampleOne_ReturnsExpectedSums()
    {
        int[] nums1 = [1, 0, 1];
        int[] nums2 = [0, 0, 0];
        int[][] queries = [[1, 1, 1], [2, 1, 0], [3, 0, 0]];

        var actual = HandleQuery(nums1, nums2, queries);

        Assert.Equal([3L], actual);
    }

    [Fact]
    public void HandleQuery_LeetCodeExampleTwo_ReturnsExpectedSums()
    {
        int[] nums1 = [1];
        int[] nums2 = [5];
        int[][] queries = [[2, 0, 0], [3, 0, 0]];

        var actual = HandleQuery(nums1, nums2, queries);

        Assert.Equal([5L], actual);
    }

    [Fact]
    public void HandleQuery_TwoFlipsOnSameRangeCancelOut_RestoresOriginalOneCount()
    {
        int[] nums1 = [1, 0, 0, 1, 0];
        int[] nums2 = [0, 0, 0, 0, 0];
        int[][] queries = [[1, 0, 3], [1, 0, 3], [2, 1, 0], [3, 0, 0]];

        var actual = HandleQuery(nums1, nums2, queries);

        // nums1 has 2 ones both before and after the two cancelling flips over [0,3].
        Assert.Equal([2L], actual);
    }

    private static long[] HandleQuery(int[] nums1, int[] nums2, int[][] queries)
    {
        var n = nums1.Length;
        var ones = new LazySegmentTree<int, bool, FlipCountOperation>(nums1);
        var sum = nums2.Sum(value => (long)value);
        var results = new List<long>();

        foreach (var query in queries)
        {
            switch (query[0])
            {
                case 1:
                    ones.UpdateRange(query[1], query[2], true);
                    break;
                case 2:
                    sum += (long)query[1] * ones.Query(0, n - 1);
                    break;
                default:
                    results.Add(sum);
                    break;
            }
        }

        return results.ToArray();
    }

    private readonly struct FlipCountOperation : IRangeUpdateOperation<int, bool>
    {
        public static int Identity => 0;

        public static bool NoUpdate => false;

        public static int Combine(int left, int right) => left + right;

        public static bool ComposeUpdate(bool outer, bool inner) => outer ^ inner;

        public static int ApplyUpdate(int aggregate, bool update, int rangeLength) => update ? rangeLength - aggregate : aggregate;
    }
}
