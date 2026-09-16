using DSAExperimentation.LeetCode.RobotBoundedInCircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotBoundedInCircle;

// Harness only. Both strategies are RobotBoundedInCircleSolution's - this file
// states LeetCode's published examples once and runs each strategy over them, so a
// failure names the strategy that broke rather than reporting a disagreement
// between two anonymous arms.
public sealed class RobotBoundedInCircleTests
{
    public static TheoryData<BoundedCircleExample> Examples =>
        new()
        {
            // LeetCode's own three examples.
            new BoundedCircleExample(Instructions: "GGLLGG", StaysWithinCircle: true),
            new BoundedCircleExample(Instructions: "GG", StaysWithinCircle: false),
            new BoundedCircleExample(Instructions: "GL", StaysWithinCircle: true),

            // Back at the origin facing north after one pass - bounded by the
            // first half of the test rather than by a changed facing.
            new BoundedCircleExample(Instructions: "GRGRGRGR", StaysWithinCircle: true),
            new BoundedCircleExample(Instructions: "RGRGRGRG", StaysWithinCircle: true),

            // Displaced but rotated 180 degrees: two passes close the loop.
            new BoundedCircleExample(Instructions: "GGRR", StaysWithinCircle: true),

            // Displaced with the rotations cancelling out - the unbounded case
            // that is not simply "G" repeated.
            new BoundedCircleExample(Instructions: "GLGLGGLGL", StaysWithinCircle: false),

            // A pure turn never leaves the origin.
            new BoundedCircleExample(Instructions: "R", StaysWithinCircle: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRobotBoundedByDirectionSwitch_LeetCodeExamples_ReturnsWhetherPathStaysWithinACircle(
        BoundedCircleExample example) =>
        Assert.Equal(
            example.StaysWithinCircle,
            RobotBoundedInCircleSolution.IsRobotBoundedByDirectionSwitch(example.Instructions));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsRobotBoundedByStepDeltaMap_LeetCodeExamples_ReturnsWhetherPathStaysWithinACircle(
        BoundedCircleExample example) =>
        Assert.Equal(
            example.StaysWithinCircle,
            RobotBoundedInCircleSolution.IsRobotBoundedByStepDeltaMap(example.Instructions));

    // One example: the instruction sequence and whether the robot stays inside a
    // circle. The expectation is named rather than carried by its position, so the
    // row reads as an assertion instead of as a bare `true`.
    public readonly record struct BoundedCircleExample(string Instructions, bool StaysWithinCircle);
}
