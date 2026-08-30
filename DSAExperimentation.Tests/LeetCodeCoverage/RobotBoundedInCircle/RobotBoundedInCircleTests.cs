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
        var stepDeltas = new HashMap<int, (int Dx, int Dy)>();
        stepDeltas.Set(0, (0, 1));
        stepDeltas.Set(1, (1, 0));
        stepDeltas.Set(2, (0, -1));
        stepDeltas.Set(3, (-1, 0));

        var direction = 0;
        var x = 0;
        var y = 0;

        foreach (var instruction in instructions)
        {
            switch (instruction)
            {
                case 'G':
                    stepDeltas.TryGetValue(direction, out var delta);
                    x += delta.Dx;
                    y += delta.Dy;
                    break;
                case 'L':
                    direction = (direction + 3) % 4;
                    break;
                case 'R':
                    direction = (direction + 1) % 4;
                    break;
            }
        }

        return (x == 0 && y == 0) || direction != 0;
    }
}
