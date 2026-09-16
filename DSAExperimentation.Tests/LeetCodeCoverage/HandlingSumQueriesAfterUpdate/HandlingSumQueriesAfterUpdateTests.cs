using DSAExperimentation.LeetCode.HandlingSumQueriesAfterUpdate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HandlingSumQueriesAfterUpdate;

// Harness only. Both strategies - the rescanning bit array and the
// LazySegmentTree<int, bool, FlipCountOperation> walk - are
// HandlingSumQueriesAfterUpdateSolution's; this file just pins them to LeetCode's
// published examples plus the cancelling-flip, multiple-reading and no-ones cases
// that exercise the flip algebra's own laws.
public sealed class HandlingSumQueriesAfterUpdateTests
{
    public static TheoryData<int[], int[], int[][], long[]> Examples =>
        new()
        {
            // LeetCode example 1: flip index 1, then add 1 per one, then read.
            { [1, 0, 1], [0, 0, 0], [[1, 1, 1], [2, 1, 0], [3, 0, 0]], [3L] },

            // LeetCode example 2: p = 0 leaves the total at nums2's own sum.
            { [1], [5], [[2, 0, 0], [3, 0, 0]], [5L] },

            // Two flips over the same range cancel: nums1 has 2 ones either way.
            { [1, 0, 0, 1, 0], [0, 0, 0, 0, 0], [[1, 0, 3], [1, 0, 3], [2, 1, 0], [3, 0, 0]], [2L] },

            // Two readings: the first sees nums2's untouched sum, the second sees it
            // after a flip of [0,1] leaves 2 ones and a type-2 query scaled by 2.
            { [0, 1, 1, 0], [1, 2, 3, 4], [[3, 0, 0], [1, 0, 1], [2, 2, 0], [3, 0, 0]], [10L, 14L] },

            // No ones at all: a type-2 query adds nothing however large p is.
            { [0, 0, 0], [1, 1, 1], [[2, 5, 0], [3, 0, 0]], [3L] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HandleQueryByArrayRescan_LeetCodeExamples_ReturnsRunningSumPerReadQuery(
        int[] nums1, int[] nums2, int[][] queries, long[] expected)
    {
        var actual = HandlingSumQueriesAfterUpdateSolution.HandleQueryByArrayRescan(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HandleQueryByLazySegmentTree_LeetCodeExamples_ReturnsRunningSumPerReadQuery(
        int[] nums1, int[] nums2, int[][] queries, long[] expected)
    {
        var actual = HandlingSumQueriesAfterUpdateSolution.HandleQueryByLazySegmentTree(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }
}
