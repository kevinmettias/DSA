using DSAExperimentation.LeetCode.MaximumSegmentSumAfterRemovals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSegmentSumAfterRemovals;

// Harness only. Both strategies are MaximumSegmentSumAfterRemovalsSolution's -
// this file pins them to LeetCode's published examples plus the degenerate shapes
// those never reach: a single element, a two-element array, an already-sorted
// suffix-first removal order, and repeated equal values where several segments
// tie for best.
//
// The rescan baseline is asserted here too, which is the point of hoisting it into
// the solution class: before this migration it lived only in the benchmark and
// nothing checked that the arm the composed strategy is measured against was even
// right.
public sealed partial class MaximumSegmentSumAfterRemovalsTests
{
    public static TheoryData<int[], int[], long[]> Examples =>
        new()
        {
            { [1, 2, 5, 6, 1], [0, 3, 2, 4, 1], [14, 7, 2, 2, 0] },
            { [3, 2, 11, 1], [3, 2, 1, 0], [16, 5, 3, 0] },
            { [42], [0], [0] },
            { [2, 3], [0, 1], [3, 0] },
            { [1, 1, 1], [1, 0, 2], [1, 1, 0] },
            { [5, 4, 3, 2, 1], [4, 3, 2, 1, 0], [14, 12, 9, 5, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSegmentSumsByRescanAfterEachRemoval_LeetCodeExamples_ReturnsBestSegmentAfterEachRemoval(
        int[] nums, int[] removeQueries, long[] expected)
    {
        var actual =
            MaximumSegmentSumAfterRemovalsSolution.MaximumSegmentSumsByRescanAfterEachRemoval(nums, removeQueries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSegmentSumsByReverseTimeDisjointSet_LeetCodeExamples_ReturnsBestSegmentAfterEachRemoval(
        int[] nums, int[] removeQueries, long[] expected)
    {
        var actual =
            MaximumSegmentSumAfterRemovalsSolution.MaximumSegmentSumsByReverseTimeDisjointSet(nums, removeQueries);

        Assert.Equal(expected, actual);
    }
}
