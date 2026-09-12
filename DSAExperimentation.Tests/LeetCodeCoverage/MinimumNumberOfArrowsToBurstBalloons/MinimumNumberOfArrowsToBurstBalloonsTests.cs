using DSAExperimentation.LeetCode.MinimumNumberOfArrowsToBurstBalloons;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfArrowsToBurstBalloons;

// Harness only: both strategies live in MinimumNumberOfArrowsToBurstBalloonsSolution
// and are asserted against the same examples, including the chained-touching case
// that rules out solving this via IntervalSet.Count.
public sealed class MinimumNumberOfArrowsToBurstBalloonsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[10, 16], [2, 8], [1, 6], [7, 12]], 2 },
            { [[1, 2], [3, 4], [5, 6], [7, 8]], 4 },
            // [1,2] and [2,3] touch, [2,3] and [3,4] touch, but no single point lies
            // in all three - proves this can't be solved by transitively merging
            // overlaps (IntervalSet.Count would wrongly return 1 here).
            { [[1, 2], [2, 3], [3, 4]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinArrowShotsByBruteForceRescan_LeetCodeExamples_ReturnsMinArrows(int[][] points, int expected) =>
        Assert.Equal(expected, MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsByBruteForceRescan(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinArrowShotsBySortEndsThenGreedyScan_LeetCodeExamples_ReturnsMinArrows(int[][] points, int expected) =>
        Assert.Equal(expected, MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsBySortEndsThenGreedyScan(points));
}
