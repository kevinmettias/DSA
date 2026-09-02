using DSAExperimentation.LeetCode.MinimumTimeVisitingAllPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeVisitingAllPoints;

// Harness only: both of MinimumTimeVisitingAllPointsSolution's strategies over
// LeetCode's examples, which is also what pins the ChebyshevHeuristic arm to the
// open-coded arithmetic it is supposed to reproduce.
public sealed class MinimumTimeVisitingAllPointsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1], [3, 4], [-1, 0]], 7 },
            { [[3, 2], [-2, 2]], 5 },
            { [[0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByInlineChebyshev_LeetCodeExamples_ReturnsSumOfChebyshevDistances(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumTimeVisitingAllPointsSolution.MinTimeByInlineChebyshev(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByPathHeuristic_LeetCodeExamples_ReturnsSumOfChebyshevDistances(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumTimeVisitingAllPointsSolution.MinTimeByPathHeuristic(points));
}
