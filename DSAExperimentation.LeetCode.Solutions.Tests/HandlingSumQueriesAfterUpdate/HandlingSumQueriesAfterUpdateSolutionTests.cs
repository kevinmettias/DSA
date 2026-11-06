using DSAExperimentation.LeetCode.Solutions.HandlingSumQueriesAfterUpdate;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HandlingSumQueriesAfterUpdate;

// Harness only. Both strategies - the rescanning bit array and the
// SegmentTree<int, bool, FlipCountOperation> walk - are
// HandlingSumQueriesAfterUpdateSolution's; this file just pins them to the
// cancelling-flip, multiple-reading and no-ones cases that exercise the flip
// algebra's own laws. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class HandlingSumQueriesAfterUpdateSolutionTests
{
    public static TheoryData<int[], int[], int[][], long[]> Cases =>
        new()
        {
            // Two flips over the same range cancel: nums1 has 2 ones either way.
            { [1, 0, 0, 1, 0], [0, 0, 0, 0, 0], [[1, 0, 3], [1, 0, 3], [2, 1, 0], [3, 0, 0]], [2L] },

            // Two readings: the first sees nums2's untouched sum, the second sees it
            // after a flip of [0,1] leaves 2 ones and a type-2 query scaled by 2.
            { [0, 1, 1, 0], [1, 2, 3, 4], [[3, 0, 0], [1, 0, 1], [2, 2, 0], [3, 0, 0]], [10L, 14L] },

            // No ones at all: a type-2 query adds nothing however large p is.
            { [0, 0, 0], [1, 1, 1], [[2, 5, 0], [3, 0, 0]], [3L] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HandleQueryByArrayRescan_Cases_ReturnsRunningSumPerReadQuery(
        int[] nums1, int[] nums2, int[][] queries, long[] expected)
    {
        var actual = HandlingSumQueriesAfterUpdateSolution.HandleQueryByArrayRescan(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HandleQueryByLazySegmentTree_Cases_ReturnsRunningSumPerReadQuery(
        int[] nums1, int[] nums2, int[][] queries, long[] expected)
    {
        var actual = HandlingSumQueriesAfterUpdateSolution.HandleQueryByLazySegmentTree(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }
}
