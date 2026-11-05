using DSAExperimentation.LeetCode.Solutions.MinimumScoreTriangulationOfPolygon;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumScoreTriangulationOfPolygon;

// Harness only: the algorithms live in MinimumScoreTriangulationOfPolygonSolution.
// One test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The case is a pentagon, which has five triangulations to
// choose between. The un-memoized baseline is asserted here too - it used to be a
// benchmark-private helper nothing checked. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MinimumScoreTriangulationOfPolygonSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 5, 6, 3, 4], 120 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinScoreTriangulationByUnmemoizedRecursion_Cases_ReturnsLowestTotalTriangleProductSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByUnmemoizedRecursion(values));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinScoreTriangulationByMemoizedRecursion_Cases_ReturnsLowestTotalTriangleProductSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByMemoizedRecursion(values));
}
