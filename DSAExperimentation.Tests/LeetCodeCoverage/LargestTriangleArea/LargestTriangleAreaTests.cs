using DSAExperimentation.LeetCode.LargestTriangleArea;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestTriangleArea;

// Harness only: both strategies live in LargestTriangleAreaSolution and are
// asserted against the same examples - LeetCode's two published ones plus the
// all-collinear case, which is the input that collapses the convex hull below
// three vertices and sends the reducing strategy down its fallback path.
public sealed partial class LargestTriangleAreaTests
{
    private const int AreaPrecision = 5;

    public static TheoryData<(int X, int Y)[], double> Examples =>
        new()
        {
            { [(0, 0), (0, 1), (1, 0), (0, 2), (2, 0)], 2.0 },
            { [(1, 0), (0, 0), (0, 1)], 0.5 },
            { [(0, 0), (1, 0), (2, 0)], 0.0 },
            { [(0, 0), (0, 3), (4, 0), (1, 1)], 6.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestAreaByAllTriples_LeetCodeExamples_ReturnsMaximumTriangleArea(
        (int X, int Y)[] points, double expected) =>
        Assert.Equal(expected, LargestTriangleAreaSolution.LargestAreaByAllTriples(points), AreaPrecision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestAreaByConvexHullReduction_LeetCodeExamples_ReturnsMaximumTriangleArea(
        (int X, int Y)[] points, double expected) =>
        Assert.Equal(
            expected,
            LargestTriangleAreaSolution.LargestAreaByConvexHullReduction(points),
            AreaPrecision);
}
