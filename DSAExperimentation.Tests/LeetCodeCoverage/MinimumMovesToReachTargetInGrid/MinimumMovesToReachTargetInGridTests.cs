using DSAExperimentation.LeetCode.MinimumMovesToReachTargetInGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumMovesToReachTargetInGrid;

// Harness only. Both strategies are MinimumMovesToReachTargetInGridSolution's - this
// file just pins them to LeetCode's published examples, including the unreachable
// target (Example 3) neither strategy can find any path to.
public sealed class MinimumMovesToReachTargetInGridTests
{
    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            { 1, 2, 5, 4, 2 },
            { 0, 1, 2, 3, 3 },
            { 1, 1, 2, 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByBoundedForwardBfs_LeetCodeExamples_ReturnsMinimumMoveCount(
        int sx, int sy, int tx, int ty, int expected) =>
        Assert.Equal(expected, MinimumMovesToReachTargetInGridSolution.MinMovesByBoundedForwardBfs(sx, sy, tx, ty));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByBackwardReduction_LeetCodeExamples_ReturnsMinimumMoveCount(
        int sx, int sy, int tx, int ty, int expected) =>
        Assert.Equal(expected, MinimumMovesToReachTargetInGridSolution.MinMovesByBackwardReduction(sx, sy, tx, ty));
}
