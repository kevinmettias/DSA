using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotBoundedInCircle;

// LeetCode 1041. Robot Bounded In Circle: this repo's own HashMap<int,(int,int)> as
// a facing-direction-to-step-delta lookup table (0=North,1=East,2=South,3=West) -
// the same move-to-delta idiom RobotReturnToOriginTests already establishes, keyed
// by direction index instead of move character since 'G' needs the *current*
// facing, not a fixed per-character delta. Running the instruction string once is
// enough: repeating it forever traces the same net rotation and displacement every
// pass, so the path stays bounded in a circle iff either the robot is back at the
// origin already, or its facing changed at all - a changed facing guarantees the
// rotations cancel out and the path closes within at most 4 passes.
public sealed partial class RobotBoundedInCircleTests
{
    [Theory]
    [InlineData("GGLLGG", true)]
    [InlineData("GG", false)]
    [InlineData("GL", true)]
    public void IsRobotBounded_LeetCodeExamples_ReturnsWhetherPathStaysWithinACircle(
        string instructions, bool expected)
        => Assert.Equal(expected, IsRobotBounded(instructions));

    private static bool IsRobotBounded(string instructions)
    {
        var stepDeltas = BuildStepDeltas();
        var state = (Direction: 0, X: 0, Y: 0);

        foreach (var instruction in instructions)
        {
            state = ApplyInstruction(stepDeltas, instruction, state);
        }

        return (state.X == 0 && state.Y == 0) || state.Direction != 0;
    }

    private static HashMap<int, (int Dx, int Dy)> BuildStepDeltas()
    {
        var stepDeltas = new HashMap<int, (int Dx, int Dy)>();
        stepDeltas.Set(0, (0, 1));
        stepDeltas.Set(1, (1, 0));
        stepDeltas.Set(2, (0, -1));
        stepDeltas.Set(3, (-1, 0));
        return stepDeltas;
    }

    private static (int Direction, int X, int Y) ApplyInstruction(
        HashMap<int, (int Dx, int Dy)> stepDeltas, char instruction, (int Direction, int X, int Y) state)
    {
        switch (instruction)
        {
            case 'G':
                stepDeltas.TryGetValue(state.Direction, out var delta);
                return (state.Direction, state.X + delta.Dx, state.Y + delta.Dy);
            case 'L':
                return ((state.Direction + 3) % 4, state.X, state.Y);
            case 'R':
                return ((state.Direction + 1) % 4, state.X, state.Y);
            default:
                return state;
        }
    }
}
