using DSAExperimentation.LeetCode.Solutions.PerfectRectangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PerfectRectangle;

// Harness only. Both strategies live in PerfectRectangleSolution: a brute-force
// pairwise overlap check and a Set<(int,int)> corner-toggle pass. The one case here
// is an overlap masked by an equal-sized gap elsewhere (caught only by the corner
// count), so both strategies are proven against the case that motivates having two
// checks. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class PerfectRectangleSolutionTests
{
    public static TheoryData<PerfectRectangleCase> Cases =>
        new()
        {
            {
                // An overlapping region (x in [1,2], y in [2,4]) is offset by an
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
    [MemberData(nameof(Cases))]
    public void IsRectangleCoverByPairwiseOverlap_Cases_ReturnsWhetherRectanglesTilePerfectly(
        PerfectRectangleCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            PerfectRectangleSolution.IsRectangleCoverByPairwiseOverlap(testCase.Rectangles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsRectangleCoverByCornerToggle_Cases_ReturnsWhetherRectanglesTilePerfectly(
        PerfectRectangleCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            PerfectRectangleSolution.IsRectangleCoverByCornerToggle(testCase.Rectangles));

    // One case: the rectangles and whether they tile their bounding box
    // with no gap and no overlap. The expected value is named at every construction
    // site, so a row reads as the case it is rather than as a bare `true` whose
    // meaning is its position. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would
    // import.
    public readonly record struct PerfectRectangleCase(int[][] Rectangles, bool Expected);
}
