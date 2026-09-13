using DSAExperimentation.LeetCode.MinimumScoreTriangulationOfPolygon;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumScoreTriangulationOfPolygon;

// Harness only: the algorithms live in MinimumScoreTriangulationOfPolygonSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke. The un-memoized baseline is asserted here
// too - it used to be a benchmark-private helper nothing checked.
public sealed class MinimumScoreTriangulationOfPolygonTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3], 6 },
            { [3, 7, 4, 5], 144 },
            { [1, 3, 1, 4, 1, 5], 13 },
            { [2, 5, 6, 3, 4], 120 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinScoreTriangulationByUnmemoizedRecursion_LeetCodeExamples_ReturnsLowestTotalTriangleProductSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByUnmemoizedRecursion(values));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinScoreTriangulationByMemoizedRecursion_LeetCodeExamples_ReturnsLowestTotalTriangleProductSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByMemoizedRecursion(values));
}
