using DSAExperimentation.LeetCode.Solutions.RobotBoundedInCircle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RobotBoundedInCircle;

// Harness only. Both strategies are RobotBoundedInCircleSolution's - this file
// states its cases once and runs each strategy over them, so a failure names the
// strategy that broke rather than reporting a disagreement between two anonymous
// arms. The cases cover paths that return to the origin, a displacement closed by a
// half turn, and displaced paths that end facing north again and so never come back.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RobotBoundedInCircleSolutionTests
{
    public static TheoryData<BoundedCircleCase> Cases =>
        new()
        {
            // Back at the origin facing north after one pass - bounded by the
            // first half of the test rather than by a changed facing.
            new BoundedCircleCase(Instructions: "GRGRGRGR", StaysWithinCircle: true),
            new BoundedCircleCase(Instructions: "RGRGRGRG", StaysWithinCircle: true),

            // Displaced but rotated 180 degrees: two passes close the loop.
            new BoundedCircleCase(Instructions: "GGRR", StaysWithinCircle: true),

            // Displaced with the rotations cancelling out - the unbounded case
            // that is not simply "G" repeated.
            new BoundedCircleCase(Instructions: "GLGLGGLGL", StaysWithinCircle: false),

            // A left turn undone by a right one: north again, but displaced. Every other
            // row reads the same with its right turns made left, so this one pins which way
            // a right turn goes.
            new BoundedCircleCase(Instructions: "GLGR", StaysWithinCircle: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsRobotBoundedByDirectionSwitch_Cases_ReturnsWhetherPathStaysWithinACircle(
        BoundedCircleCase testCase) =>
        Assert.Equal(
            testCase.StaysWithinCircle,
            RobotBoundedInCircleSolution.IsRobotBoundedByDirectionSwitch(testCase.Instructions));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsRobotBoundedByStepDeltaMap_Cases_ReturnsWhetherPathStaysWithinACircle(
        BoundedCircleCase testCase) =>
        Assert.Equal(
            testCase.StaysWithinCircle,
            RobotBoundedInCircleSolution.IsRobotBoundedByStepDeltaMap(testCase.Instructions));

    // One case: the instruction sequence and whether the robot stays inside a
    // circle. The expectation is named rather than carried by its position, so the
    // row reads as an assertion instead of as a bare `true`.
    public readonly record struct BoundedCircleCase(string Instructions, bool StaysWithinCircle);
}
