using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfArrowsToBurstBalloons;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfArrowsToBurstBalloons;

// Harness only: both strategies live in MinimumNumberOfArrowsToBurstBalloonsSolution
// and are asserted against the same cases, among them the chained-touching case
// that rules out solving this via IntervalSet.Count.
public sealed partial class MinimumNumberOfArrowsToBurstBalloonsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // [1,2] and [2,3] touch, [2,3] and [3,4] touch, but no single point lies
            // in all three - proves this can't be solved by transitively merging
            // overlaps (IntervalSet.Count would wrongly return 1 here).
            { [[1, 2], [2, 3], [3, 4]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMinArrowShotsByBruteForceRescan_Cases_ReturnsMinArrows(int[][] points, int expected) =>
        Assert.Equal(expected, MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsByBruteForceRescan(points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMinArrowShotsBySortEndsThenGreedyScan_Cases_ReturnsMinArrows(int[][] points, int expected) =>
        Assert.Equal(expected, MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsBySortEndsThenGreedyScan(points));
}
