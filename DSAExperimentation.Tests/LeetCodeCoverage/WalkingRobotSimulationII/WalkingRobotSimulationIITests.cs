using static DSAExperimentation.LeetCode.WalkingRobotSimulationII.WalkingRobotSimulationIISolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WalkingRobotSimulationII;

// Harness only: both strategies live in WalkingRobotSimulationIISolution. LeetCode's
// own shape here is a stateful object queried across a sequence of calls, so an
// example is a grid, the Move script run against it, and the position and heading
// the robot ends on - GetPos and GetDir are pure, so a mid-script checkpoint is
// simply another row with the shorter prefix of the same script.
public sealed class WalkingRobotSimulationIITests
{
    public static TheoryData<int, int, int[], int, int, string> Examples =>
        new()
        {
            // LeetCode's published example, at both of the points it queries: after
            // step(2) + step(2), then after three more steps of 2, 1 and 4.
            { 6, 3, [2, 2], 4, 0, "East" },
            { 6, 3, [2, 2, 2, 1, 4], 1, 2, "West" },

            // Turning the first two corners: the last cell of the east edge still
            // reads as facing east, and the far corner as facing north.
            { 6, 3, [5], 5, 0, "East" },
            { 6, 3, [2, 2, 2], 5, 1, "North" },
            { 6, 3, [7], 5, 2, "North" },
            { 6, 3, [2, 2, 2, 3, 3, 3, 3, 3], 5, 2, "North" },

            // The origin is the one cell that answers two headings: east before the
            // robot has moved, south once a whole loop of 2*((6-1)+(3-1)) has
            // brought it back - and that stays true after any number of loops.
            { 6, 3, [], 0, 0, "East" },
            { 6, 3, [14], 0, 0, "South" },
            { 6, 3, [14, 14], 0, 0, "South" },

            // Past a full loop the walk simply resumes along the east edge.
            { 6, 3, [17], 3, 0, "East" },

            // A grid only two cells wide, where the east and west edges are a single
            // step each.
            { 2, 3, [4], 0, 2, "West" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobotByStepSimulation_LeetCodeExamples_EndsOnExpectedCellAndHeading(
        int width, int height, int[] moves, int expectedX, int expectedY, string expectedDirection) =>
        AssertEndState(new RobotByStepSimulation(width, height), moves, expectedX, expectedY, expectedDirection);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobotByPerimeterFormula_LeetCodeExamples_EndsOnExpectedCellAndHeading(
        int width, int height, int[] moves, int expectedX, int expectedY, string expectedDirection) =>
        AssertEndState(new RobotByPerimeterFormula(width, height), moves, expectedX, expectedY, expectedDirection);

    private static void AssertEndState(
        IRobot robot, int[] moves, int expectedX, int expectedY, string expectedDirection)
    {
        foreach (var steps in moves)
        {
            robot.Move(steps);
        }

        Assert.Equal((expectedX, expectedY), robot.GetPos());
        Assert.Equal(expectedDirection, robot.GetDir());
    }
}
