using DSAExperimentation.LeetCode.RectangleOverlap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleOverlap;

// Harness only: both strategies are RectangleOverlapSolution's - the closed-form
// O(1) axis-interval check and the unit-grid brute force it has to beat, now under
// test itself instead of sitting unasserted as a benchmark-only baseline.
public sealed class RectangleOverlapTests
{
    public static TheoryData<int[], int[], bool> Examples =>
        new()
        {
            // LeetCode's first example: rec1 = [0,0,2,2], rec2 = [1,1,3,3] -> true
            { [0, 0, 2, 2], [1, 1, 3, 3], true },
            // LeetCode's second example: rec1 = [0,0,1,1], rec2 = [1,0,2,1] -> false
            // (they touch along an edge, which is zero overlap area)
            { [0, 0, 1, 1], [1, 0, 2, 1], false },
            // LeetCode's third example: rec1 = [0,0,1,1], rec2 = [2,2,3,3] -> false
            { [0, 0, 1, 1], [2, 2, 3, 3], false },
            // One rectangle entirely inside the other
            { [0, 0, 10, 10], [3, 3, 5, 5], true },
            // Touching at a single corner only: still zero overlap area
            { [0, 0, 1, 1], [1, 1, 2, 2], false },
            // Overlapping on x but disjoint on y: both axes must overlap
            { [0, 0, 4, 1], [1, 5, 3, 8], false },
            // Negative coordinates: the bounding box the grid arm allocates has to
            // be translated, not indexed from the origin
            { [-3, -3, -1, -1], [-2, -2, 0, 0], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OverlapsByClosedFormAxisIntervals_LeetCodeExamples_ReturnsExpectedOverlap(
        int[] rec1, int[] rec2, bool expected) =>
        Assert.Equal(expected, RectangleOverlapSolution.OverlapsByClosedFormAxisIntervals(rec1, rec2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void OverlapsByUnitGridIntersectionScan_LeetCodeExamples_ReturnsExpectedOverlap(
        int[] rec1, int[] rec2, bool expected) =>
        Assert.Equal(expected, RectangleOverlapSolution.OverlapsByUnitGridIntersectionScan(rec1, rec2));
}
