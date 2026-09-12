using DSAExperimentation.LeetCode.ZumaGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ZumaGame;

// Harness only. Both strategies are ZumaGameSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class ZumaGameTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "WRRBBW", "RB", -1 },
            { "WWRRBBWW", "WRBRW", 2 },
            { "G", "GGGGG", 2 },
            { "RBYYBBRRB", "YRBGB", 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinStepByBruteForceDfs_LeetCodeExamples_ReturnsMinimumBallsNeededOrNegativeOne(
        string board, string hand, int expected) =>
        Assert.Equal(expected, ZumaGameSolution.FindMinStepByBruteForceDfs(board, hand));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinStepByQueueBfsDedup_LeetCodeExamples_ReturnsMinimumBallsNeededOrNegativeOne(
        string board, string hand, int expected) =>
        Assert.Equal(expected, ZumaGameSolution.FindMinStepByQueueBfsDedup(board, hand));
}
