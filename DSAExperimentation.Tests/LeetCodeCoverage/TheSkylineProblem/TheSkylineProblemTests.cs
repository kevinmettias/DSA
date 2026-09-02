using DSAExperimentation.LeetCode.TheSkylineProblem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheSkylineProblem;

// Harness only: both strategies live in TheSkylineProblemSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names
// the strategy that broke. The third example has an ending building and a starting
// building of equal height sharing an x-coordinate, proving intra-batch event
// order doesn't affect the result.
public sealed class TheSkylineProblemTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [[2, 9, 10], [3, 7, 15], [5, 12, 12], [15, 20, 10], [19, 24, 8]],
                [[2, 10], [3, 15], [7, 12], [12, 0], [15, 10], [20, 8], [24, 0]]
            },
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
    [MemberData(nameof(Examples))]
    public void GetSkylineByBruteForce_LeetCodeExamples_ReturnsKeyPoints(int[][] buildings, int[][] expected) =>
        Assert.Equal(expected, TheSkylineProblemSolution.GetSkylineByBruteForce(buildings));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetSkylineBySweepLineHeap_LeetCodeExamples_ReturnsKeyPoints(int[][] buildings, int[][] expected) =>
        Assert.Equal(expected, TheSkylineProblemSolution.GetSkylineBySweepLineHeap(buildings));
}
