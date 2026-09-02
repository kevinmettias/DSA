using DSAExperimentation.LeetCode.PerfectRectangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PerfectRectangle;

// Harness only. Both strategies live in PerfectRectangleSolution: a brute-force
// pairwise overlap check and a Set<(int,int)> corner-toggle pass. LeetCode's own
// three examples cover a gap (caught by the area check alone) and an overlap
// masked by an equal-sized gap elsewhere (caught only by the corner count), so both
// strategies are proven against the same cases that motivate having two checks.
public sealed class PerfectRectangleTests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            {
                [
                    [1, 1, 3, 3],
                    [3, 1, 4, 2],
                    [3, 2, 4, 4],
                    [1, 3, 2, 4],
                    [2, 3, 3, 4],
                ],
                true
            },
            {
                // LeetCode's own second example: total area (6) falls short of the
                // bounding box's area (9), so the area check alone catches this one.
                [
                    [1, 1, 2, 3],
                    [1, 3, 2, 4],
                    [3, 1, 4, 2],
                    [3, 2, 4, 4],
                ],
                false
            },
            {
                // A deliberately harder case than the gap example above: an
                // overlapping region (x in [1,2], y in [2,4]) is offset by an
                // equal-area gap elsewhere (x in [3,4], y in [2,4]), so total area
                // (16) still equals the 4x4 bounding box's area (16) - the area
                // check alone cannot catch this, only the corner count (10
                // survivors here, not 4) can.
                [
                    [0, 0, 2, 4],
                    [2, 0, 4, 2],
                    [1, 2, 3, 4],
                ],
                false
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRectangleCoverByPairwiseOverlap_Examples_ReturnsWhetherRectanglesTilePerfectly(
        int[][] rectangles, bool expected) =>
        Assert.Equal(expected, PerfectRectangleSolution.IsRectangleCoverByPairwiseOverlap(rectangles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRectangleCoverByCornerToggle_Examples_ReturnsWhetherRectanglesTilePerfectly(
        int[][] rectangles, bool expected) =>
        Assert.Equal(expected, PerfectRectangleSolution.IsRectangleCoverByCornerToggle(rectangles));
}
