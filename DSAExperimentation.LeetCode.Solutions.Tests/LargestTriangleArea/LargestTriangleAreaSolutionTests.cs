using DSAExperimentation.LeetCode.Solutions.LargestTriangleArea;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LargestTriangleArea;

// Harness only: both strategies live in LargestTriangleAreaSolution and are
// asserted against the same cases - LeetCode's two published ones plus the
// all-collinear case, which is the input that collapses the convex hull below
// three vertices and sends the reducing strategy down its fallback path.
public sealed partial class LargestTriangleAreaSolutionTests
{
    private const int AreaPrecision = 5;

    public static TheoryData<(int X, int Y)[], double> Cases =>
        new()
        {
            { [(0, 0), (1, 0), (2, 0)], 0.0 },
            { [(0, 0), (0, 3), (4, 0), (1, 1)], 6.0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestAreaByAllTriples_Cases_ReturnsMaximumTriangleArea(
        (int X, int Y)[] points, double expected) =>
        Assert.Equal(expected, LargestTriangleAreaSolution.LargestAreaByAllTriples(points), AreaPrecision);

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestAreaByConvexHullReduction_Cases_ReturnsMaximumTriangleArea(
        (int X, int Y)[] points, double expected) =>
        Assert.Equal(
            expected,
            LargestTriangleAreaSolution.LargestAreaByConvexHullReduction(points),
            AreaPrecision);
}
