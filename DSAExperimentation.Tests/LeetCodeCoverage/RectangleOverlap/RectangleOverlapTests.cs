using DSAExperimentation.LeetCode.RectangleOverlap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleOverlap;

// Harness only: both strategies are RectangleOverlapSolution's - the closed-form
// O(1) axis-interval check and the unit-grid brute force it has to beat, now under
// test itself instead of sitting unasserted as a benchmark-only baseline.
public sealed class RectangleOverlapTests
{
    public static TheoryData<RectangleOverlapCase> Examples =>
        new()
        {
            // LeetCode's first example: rec1 = [0,0,2,2], rec2 = [1,1,3,3] -> true
            { new RectangleOverlapCase(Rec1: [0, 0, 2, 2], Rec2: [1, 1, 3, 3], Expected: true) },
            // LeetCode's second example: rec1 = [0,0,1,1], rec2 = [1,0,2,1] -> false
            // (they touch along an edge, which is zero overlap area)
            { new RectangleOverlapCase(Rec1: [0, 0, 1, 1], Rec2: [1, 0, 2, 1], Expected: false) },
            // LeetCode's third example: rec1 = [0,0,1,1], rec2 = [2,2,3,3] -> false
            { new RectangleOverlapCase(Rec1: [0, 0, 1, 1], Rec2: [2, 2, 3, 3], Expected: false) },
            // One rectangle entirely inside the other
            { new RectangleOverlapCase(Rec1: [0, 0, 10, 10], Rec2: [3, 3, 5, 5], Expected: true) },
            // Touching at a single corner only: still zero overlap area
            { new RectangleOverlapCase(Rec1: [0, 0, 1, 1], Rec2: [1, 1, 2, 2], Expected: false) },
            // Overlapping on x but disjoint on y: both axes must overlap
            { new RectangleOverlapCase(Rec1: [0, 0, 4, 1], Rec2: [1, 5, 3, 8], Expected: false) },
            // Negative coordinates: the bounding box the grid arm allocates has to
            // be translated, not indexed from the origin
            { new RectangleOverlapCase(Rec1: [-3, -3, -1, -1], Rec2: [-2, -2, 0, 0], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OverlapsByClosedFormAxisIntervals_LeetCodeExamples_ReturnsExpectedOverlap(
        RectangleOverlapCase example)
    {
        var actual = RectangleOverlapSolution.OverlapsByClosedFormAxisIntervals(example.Rec1, example.Rec2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OverlapsByUnitGridIntersectionScan_LeetCodeExamples_ReturnsExpectedOverlap(
        RectangleOverlapCase example)
    {
        var actual = RectangleOverlapSolution.OverlapsByUnitGridIntersectionScan(example.Rec1, example.Rec2);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two rectangles and whether they overlap with a
    // positive area. The expected value is named at every construction site, so a row
    // reads as the case it is rather than as a bare `true` whose meaning is its
    // position. Nested because it is only ever used inside this test class - it is
    // this harness's own vocabulary, not a type another file would import.
    public readonly record struct RectangleOverlapCase(int[] Rec1, int[] Rec2, bool Expected);
}
