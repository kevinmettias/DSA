using DSAExperimentation.LeetCode.PerfectRectangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PerfectRectangle;

// Harness only. Both strategies live in PerfectRectangleSolution: a brute-force
// pairwise overlap check and a Set<(int,int)> corner-toggle pass. LeetCode's own
// three examples cover a gap (caught by the area check alone) and an overlap
// masked by an equal-sized gap elsewhere (caught only by the corner count), so both
// strategies are proven against the same cases that motivate having two checks.
public sealed class PerfectRectangleTests
{
    public static TheoryData<PerfectRectangleCase> Examples =>
        new()
        {
            {
                new PerfectRectangleCase(
                    [
                        [1, 1, 3, 3],
                        [3, 1, 4, 2],
                        [3, 2, 4, 4],
                        [1, 3, 2, 4],
                        [2, 3, 3, 4],
                    ],
                    Expected: true)
            },
            {
                // LeetCode's own second example: total area (6) falls short of the
                // bounding box's area (9), so the area check alone catches this one.
                new PerfectRectangleCase(
                    [
                        [1, 1, 2, 3],
                        [1, 3, 2, 4],
                        [3, 1, 4, 2],
                        [3, 2, 4, 4],
                    ],
                    Expected: false)
            },
            {
                // A deliberately harder case than the gap example above: an
                // overlapping region (x in [1,2], y in [2,4]) is offset by an
                // equal-area gap elsewhere (x in [3,4], y in [2,4]), so total area
                // (16) still equals the 4x4 bounding box's area (16) - the area
                // check alone cannot catch this, only the corner count (10
                // survivors here, not 4) can.
                new PerfectRectangleCase(
                    [
                        [0, 0, 2, 4],
                        [2, 0, 4, 2],
                        [1, 2, 3, 4],
                    ],
                    Expected: false)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRectangleCoverByPairwiseOverlap_Examples_ReturnsWhetherRectanglesTilePerfectly(
        PerfectRectangleCase example) =>
        Assert.Equal(
            example.Expected,
            PerfectRectangleSolution.IsRectangleCoverByPairwiseOverlap(example.Rectangles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRectangleCoverByCornerToggle_Examples_ReturnsWhetherRectanglesTilePerfectly(
        PerfectRectangleCase example) =>
        Assert.Equal(
            example.Expected,
            PerfectRectangleSolution.IsRectangleCoverByCornerToggle(example.Rectangles));

    // One LeetCode example: the rectangles and whether they tile their bounding box
    // with no gap and no overlap. The expected value is named at every construction
    // site, so a row reads as the case it is rather than as a bare `true` whose
    // meaning is its position. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would
    // import.
    public readonly record struct PerfectRectangleCase(int[][] Rectangles, bool Expected);
}
