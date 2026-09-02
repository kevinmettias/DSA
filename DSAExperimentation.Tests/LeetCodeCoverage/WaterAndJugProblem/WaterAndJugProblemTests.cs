using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WaterAndJugProblem;

// LeetCode 365. Water and Jug Problem: DepthFirstSearch.Traverse over an implicit
// graph of (jugX, jugY) fill states - the same "chess position, generated on
// demand" composition ExpressionAddOperatorsTests already uses for this exact
// primitive. Each state's six successors are the classic fill/empty/pour moves;
// the target is reachable iff some visited state's two jugs sum to it (pouring
// either jug out entirely is itself one of the six moves, so "target sitting in
// one jug" is already covered by that jug summing with an empty other jug).
public sealed partial class WaterAndJugProblemTests
{
    [Theory]
    [InlineData(3, 5, 4, true)]
    [InlineData(2, 6, 5, false)]
    [InlineData(1, 2, 3, true)]
    [InlineData(2, 3, 0, true)]
    public void CanMeasureWater_LeetCodeExamples_MatchesExpectedReachability(int jugX, int jugY, int target, bool expected)
    {
        var actual = CanMeasureWater(jugX, jugY, target);
        Assert.Equal(expected, actual);
    }

    private static bool CanMeasureWater(int jugX, int jugY, int target)
    {
        if (target > jugX + jugY)
        {
            return false;
        }

        var start = (X: 0, Y: 0);
        var visited = DepthFirstSearch.Traverse(start, state => Successors(state, jugX, jugY));

        return visited.Any(state => state.X + state.Y == target);
    }

    private static IEnumerable<(int X, int Y)> Successors((int X, int Y) state, int jugX, int jugY)
    {
        yield return (jugX, state.Y);
        yield return (state.X, jugY);
        yield return (0, state.Y);
        yield return (state.X, 0);

        var pourXtoY = Math.Min(state.X, jugY - state.Y);
        yield return (state.X - pourXtoY, state.Y + pourXtoY);

        var pourYtoX = Math.Min(state.Y, jugX - state.X);
        yield return (state.X + pourYtoX, state.Y - pourYtoX);
    }
}
