using DSAExperimentation.LeetCode.ZumaGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ZumaGame;

// Harness only. Both strategies are ZumaGameSolution's - this file just pins them to
// LeetCode's published examples. The board and the hand are both strings and the
// problem is not symmetric in them, so each row names which is which rather than
// leaving two interchangeable positions.
public sealed class ZumaGameTests
{
    public static TheoryData<ZumaExample> Examples =>
        new()
        {
            { new ZumaExample(Board: "WRRBBW", Hand: "RB", Expected: -1) },
            { new ZumaExample(Board: "WWRRBBWW", Hand: "WRBRW", Expected: 2) },
            { new ZumaExample(Board: "G", Hand: "GGGGG", Expected: 2) },
            { new ZumaExample(Board: "RBYYBBRRB", Hand: "YRBGB", Expected: 3) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinStepByBruteForceDfs_LeetCodeExamples_ReturnsMinimumBallsNeededOrNegativeOne(
        ZumaExample example)
    {
        var actual = ZumaGameSolution.FindMinStepByBruteForceDfs(
            new BallBoard(example.Board), new BallHand(example.Hand));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinStepByQueueBfsDedup_LeetCodeExamples_ReturnsMinimumBallsNeededOrNegativeOne(
        ZumaExample example)
    {
        var actual = ZumaGameSolution.FindMinStepByQueueBfsDedup(
            new BallBoard(example.Board), new BallHand(example.Hand));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the row of balls on the board and the balls still in hand.
    // The two are the same type and the search does not treat them alike, so the row
    // names which is which rather than leaving two interchangeable positions. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct ZumaExample(string Board, string Hand, int Expected);
}
