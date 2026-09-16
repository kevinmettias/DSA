using DSAExperimentation.LeetCode.RectangleArea;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleArea;

// Harness only: both strategies are RectangleAreaSolution's - the closed-form O(1)
// overlap arithmetic and the unit-grid brute force it has to beat, now under test
// itself instead of sitting unasserted as a benchmark-only baseline.
public sealed class RectangleAreaTests
{
    public static TheoryData<RectangleAreaCase> Examples =>
        new()
        {
            // LeetCode's own example: (-3,0,3,4) and (0,-1,9,2) -> 45
            { new RectangleAreaCase(Ax1: -3, Ay1: 0, Ax2: 3, Ay2: 4, Bx1: 0, By1: -1, Bx2: 9, By2: 2, Expected: 45) },
            // Second example: (-2,-2,2,2) and (-2,-2,2,2) fully coincide -> single area
            { new RectangleAreaCase(Ax1: -2, Ay1: -2, Ax2: 2, Ay2: 2, Bx1: -2, By1: -2, Bx2: 2, By2: 2, Expected: 16) },
            // Disjoint rectangles: no overlap to subtract
            { new RectangleAreaCase(Ax1: 0, Ay1: 0, Ax2: 2, Ay2: 2, Bx1: 10, By1: 10, Bx2: 12, By2: 12, Expected: 8) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaByClosedFormOverlap_LeetCodeExamples_ReturnsExpectedArea(
        RectangleAreaCase example)
    {
        var first = new Rectangle(example.Ax1, example.Ay1, example.Ax2, example.Ay2);
        var second = new Rectangle(example.Bx1, example.By1, example.Bx2, example.By2);
        var area = RectangleAreaSolution.TotalAreaByClosedFormOverlap(first, second);

        Assert.Equal(example.Expected, area);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaByUnitGridCoverageCount_LeetCodeExamples_ReturnsExpectedArea(
        RectangleAreaCase example)
    {
        var first = new Rectangle(example.Ax1, example.Ay1, example.Ax2, example.Ay2);
        var second = new Rectangle(example.Bx1, example.By1, example.Bx2, example.By2);
        var area = RectangleAreaSolution.TotalAreaByUnitGridCoverageCount(first, second);

        Assert.Equal(example.Expected, area);
    }

    // One LeetCode example: the two rectangles as their own corners and the total area
    // they cover. All eight coordinates are bare ints and four of them are x's, so each
    // is named at every construction site and a row reads as the case it is rather than
    // as eight positions a caller has to keep in order. The coordinates stay plain ints
    // rather than the solution's own Rectangle because Rectangle is internal to the
    // solution assembly and so cannot appear in a public TheoryData. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct RectangleAreaCase(
        int Ax1, int Ay1, int Ax2, int Ay2, int Bx1, int By1, int Bx2, int By2, long Expected);
}
