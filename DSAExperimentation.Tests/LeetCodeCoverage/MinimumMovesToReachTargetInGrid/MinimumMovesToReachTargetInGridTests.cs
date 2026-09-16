using DSAExperimentation.LeetCode.MinimumMovesToReachTargetInGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumMovesToReachTargetInGrid;

// Harness only. Both strategies are MinimumMovesToReachTargetInGridSolution's - this
// file just pins them to LeetCode's published examples, including the unreachable
// target (Example 3) neither strategy can find any path to.
public sealed class MinimumMovesToReachTargetInGridTests
{
    public static TheoryData<MoveCountExample> Examples =>
        new()
        {
            { new MoveCountExample(Source: (1, 2), Target: (5, 4), Expected: 2) },
            { new MoveCountExample(Source: (0, 1), Target: (2, 3), Expected: 3) },
            { new MoveCountExample(Source: (1, 1), Target: (2, 2), Expected: -1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByBoundedForwardBfs_LeetCodeExamples_ReturnsMinimumMoveCount(MoveCountExample example)
    {
        var actual = MinimumMovesToReachTargetInGridSolution.MinMovesByBoundedForwardBfs(
            example.Source.X, example.Source.Y, example.Target.X, example.Target.Y);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByBackwardReduction_LeetCodeExamples_ReturnsMinimumMoveCount(MoveCountExample example)
    {
        var actual = MinimumMovesToReachTargetInGridSolution.MinMovesByBackwardReduction(
            example.Source.X, example.Source.Y, example.Target.X, example.Target.Y);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the point the walk starts from, the point it has to
    // reach, and the fewest moves that get there. The four coordinates are all `int`,
    // so the fields name which point each pair belongs to rather than leaving a row
    // of bare numbers where the start and the target are a transposition apart.
    public readonly record struct MoveCountExample(
        (int X, int Y) Source,
        (int X, int Y) Target,
        int Expected);
}
