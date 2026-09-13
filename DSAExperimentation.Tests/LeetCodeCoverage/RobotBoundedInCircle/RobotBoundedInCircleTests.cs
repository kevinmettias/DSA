using DSAExperimentation.LeetCode.RobotBoundedInCircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotBoundedInCircle;

// Harness only. Both strategies are RobotBoundedInCircleSolution's - this file
// states LeetCode's published examples once and runs each strategy over them, so a
// failure names the strategy that broke rather than reporting a disagreement
// between two anonymous arms.
public sealed class RobotBoundedInCircleTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            // LeetCode's own three examples.
            { "GGLLGG", true },
            { "GG", false },
            { "GL", true },

            // Back at the origin facing north after one pass - bounded by the
            // first half of the test rather than by a changed facing.
            { "GRGRGRGR", true },
            { "RGRGRGRG", true },

            // Displaced but rotated 180 degrees: two passes close the loop.
            { "GGRR", true },

            // Displaced with the rotations cancelling out - the unbounded case
            // that is not simply "G" repeated.
            { "GLGLGGLGL", false },

            // A pure turn never leaves the origin.
            { "R", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRobotBoundedByDirectionSwitch_LeetCodeExamples_ReturnsWhetherPathStaysWithinACircle(
        string instructions, bool expected) =>
        Assert.Equal(expected, RobotBoundedInCircleSolution.IsRobotBoundedByDirectionSwitch(instructions));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRobotBoundedByStepDeltaMap_LeetCodeExamples_ReturnsWhetherPathStaysWithinACircle(
        string instructions, bool expected) =>
        Assert.Equal(expected, RobotBoundedInCircleSolution.IsRobotBoundedByStepDeltaMap(instructions));
}
