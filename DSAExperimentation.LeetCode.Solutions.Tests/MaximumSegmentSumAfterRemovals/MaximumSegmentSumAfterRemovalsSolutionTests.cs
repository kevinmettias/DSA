using DSAExperimentation.LeetCode.Solutions.MaximumSegmentSumAfterRemovals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumSegmentSumAfterRemovals;

// Harness only. Both strategies are MaximumSegmentSumAfterRemovalsSolution's -
// this file pins them to the degenerate shapes
// those never reach: a single element, a two-element array, an already-sorted
// suffix-first removal order, and repeated equal values where several segments
// tie for best.
//
// The rescan baseline is asserted here too, which is the point of hoisting it into
// the solution class: before this migration it lived only in the benchmark and
// nothing checked that the arm the composed strategy is measured against was even
// right.
public sealed partial class MaximumSegmentSumAfterRemovalsSolutionTests
{
    public static TheoryData<int[], int[], long[]> Cases =>
        new()
        {
            { [42], [0], [0] },
            { [2, 3], [0, 1], [3, 0] },
            { [1, 1, 1], [1, 0, 2], [1, 1, 0] },
            { [5, 4, 3, 2, 1], [4, 3, 2, 1, 0], [14, 12, 9, 5, 0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSegmentSumsByRescanAfterEachRemoval_Cases_ReturnsBestSegmentAfterEachRemoval(
        int[] nums, int[] removeQueries, long[] expected)
    {
        var actual =
            MaximumSegmentSumAfterRemovalsSolution.MaximumSegmentSumsByRescanAfterEachRemoval(nums, removeQueries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSegmentSumsByReverseTimeDisjointSet_Cases_ReturnsBestSegmentAfterEachRemoval(
        int[] nums, int[] removeQueries, long[] expected)
    {
        var actual =
            MaximumSegmentSumAfterRemovalsSolution.MaximumSegmentSumsByReverseTimeDisjointSet(nums, removeQueries);

        Assert.Equal(expected, actual);
    }
}
