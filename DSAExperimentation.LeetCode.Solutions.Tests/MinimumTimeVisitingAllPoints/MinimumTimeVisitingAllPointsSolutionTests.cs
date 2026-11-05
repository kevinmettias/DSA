using DSAExperimentation.LeetCode.Solutions.MinimumTimeVisitingAllPoints;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumTimeVisitingAllPoints;

// Harness only: both of MinimumTimeVisitingAllPointsSolution's strategies over a
// single point, which takes no time to visit. LeetCode's published examples run in
// CatalogCaseTests, and they are what pins the ChebyshevHeuristic arm to the
// open-coded arithmetic it is supposed to reproduce.
public sealed partial class MinimumTimeVisitingAllPointsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinTimeByInlineChebyshev_Cases_ReturnsSumOfChebyshevDistances(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumTimeVisitingAllPointsSolution.MinTimeByInlineChebyshev(points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinTimeByPathHeuristic_Cases_ReturnsSumOfChebyshevDistances(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumTimeVisitingAllPointsSolution.MinTimeByPathHeuristic(points));
}
