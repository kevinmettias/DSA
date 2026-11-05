using DSAExperimentation.LeetCode.Solutions.TheSkylineProblem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheSkylineProblem;

// Harness only: both strategies live in TheSkylineProblemSolution. One test method
// per strategy over one shared set of cases, so a failure names the strategy that
// broke. The first case is a single building; the second has an ending building and
// a starting building of equal height sharing an x-coordinate, proving intra-batch
// event order doesn't affect the result. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class TheSkylineProblemSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            {
                [[0, 2, 3]],
                [[0, 3], [2, 0]]
            },
            {
                [[1, 5, 4], [5, 10, 4]],
                [[1, 4], [10, 0]]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetSkylineByBruteForce_Cases_ReturnsKeyPoints(int[][] buildings, int[][] expected) =>
        Assert.Equal(expected, TheSkylineProblemSolution.GetSkylineByBruteForce(buildings));

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetSkylineBySweepLineHeap_Cases_ReturnsKeyPoints(int[][] buildings, int[][] expected) =>
        Assert.Equal(expected, TheSkylineProblemSolution.GetSkylineBySweepLineHeap(buildings));
}
