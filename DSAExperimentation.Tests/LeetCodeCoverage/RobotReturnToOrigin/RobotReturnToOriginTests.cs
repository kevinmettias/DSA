using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotReturnToOrigin;

// LeetCode 657. Robot Return to Origin: this repo's own HashMap<char,(int,int)> as
// a move-to-displacement lookup table, folded over the move string to net out the
// (x, y) drift instead of a hand-written switch per character (the switch version
// is RobotReturnToOriginBenchmarks' baseline).
public sealed partial class RobotReturnToOriginTests
{
    [Theory]
    [InlineData("UD", true)]
    [InlineData("LL", false)]
    [InlineData("RRDD", false)]
    [InlineData("LDRRLRUULR", false)]
    public void JudgeCircle_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(string moves, bool expected)
        => Assert.Equal(expected, JudgeCircle(moves));

    private static bool JudgeCircle(string moves)
    {
        var deltas = new HashMap<char, (int Dx, int Dy)>();
        deltas.Set('U', (0, 1));
        deltas.Set('D', (0, -1));
        deltas.Set('L', (-1, 0));
        deltas.Set('R', (1, 0));

        var x = 0;
        var y = 0;

        foreach (var move in moves)
        {
            if (deltas.TryGetValue(move, out var delta))
            {
                x += delta.Dx;
                y += delta.Dy;
            }
        }

        return x == 0 && y == 0;
    }
}
