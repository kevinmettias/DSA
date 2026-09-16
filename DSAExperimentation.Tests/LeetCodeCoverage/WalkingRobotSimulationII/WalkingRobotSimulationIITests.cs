using DSAExperimentation.LeetCode.WalkingRobotSimulationII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WalkingRobotSimulationII;

// Harness only: both strategies live in WalkingRobotSimulationIISolution. LeetCode's
// own shape here is a stateful object queried across a sequence of calls, so an
// example is a grid, the Move script run against it, and the position and heading
// the robot ends on - GetPosition and GetDirection are pure, so a mid-script checkpoint is
// simply another row with the shorter prefix of the same script.
public sealed partial class WalkingRobotSimulationIITests
{
    public static TheoryData<RobotExample> Examples =>
        new()
        {
            // LeetCode's published example, at both of the points it queries: after
            // step(2) + step(2), then after three more steps of 2, 1 and 4.
            { new RobotExample(Width: 6, Height: 3, Moves: [2, 2], ExpectedX: 4, ExpectedY: 0, ExpectedDirection: "East") },
            { new RobotExample(Width: 6, Height: 3, Moves: [2, 2, 2, 1, 4], ExpectedX: 1, ExpectedY: 2, ExpectedDirection: "West") },

            // Turning the first two corners: the last cell of the east edge still
            // reads as facing east, and the far corner as facing north.
            { new RobotExample(Width: 6, Height: 3, Moves: [5], ExpectedX: 5, ExpectedY: 0, ExpectedDirection: "East") },
            { new RobotExample(Width: 6, Height: 3, Moves: [2, 2, 2], ExpectedX: 5, ExpectedY: 1, ExpectedDirection: "North") },
            { new RobotExample(Width: 6, Height: 3, Moves: [7], ExpectedX: 5, ExpectedY: 2, ExpectedDirection: "North") },
            { new RobotExample(Width: 6, Height: 3, Moves: [2, 2, 2, 3, 3, 3, 3, 3], ExpectedX: 5, ExpectedY: 2, ExpectedDirection: "North") },

            // The origin is the one cell that answers two headings: east before the
            // robot has moved, south once a whole loop of 2*((6-1)+(3-1)) has
            // brought it back - and that stays true after any number of loops.
            { new RobotExample(Width: 6, Height: 3, Moves: [], ExpectedX: 0, ExpectedY: 0, ExpectedDirection: "East") },
            { new RobotExample(Width: 6, Height: 3, Moves: [14], ExpectedX: 0, ExpectedY: 0, ExpectedDirection: "South") },
            { new RobotExample(Width: 6, Height: 3, Moves: [14, 14], ExpectedX: 0, ExpectedY: 0, ExpectedDirection: "South") },

            // Past a full loop the walk simply resumes along the east edge.
            { new RobotExample(Width: 6, Height: 3, Moves: [17], ExpectedX: 3, ExpectedY: 0, ExpectedDirection: "East") },

            // A grid only two cells wide, where the east and west edges are a single
            // step each.
            { new RobotExample(Width: 2, Height: 3, Moves: [4], ExpectedX: 0, ExpectedY: 2, ExpectedDirection: "West") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobotByStepSimulation_LeetCodeExamples_EndsOnExpectedCellAndHeading(RobotExample example) =>
        AssertEndState(new WalkingRobotSimulationIISolution.RobotByStepSimulation(example.Width, example.Height), example);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobotByPerimeterFormula_LeetCodeExamples_EndsOnExpectedCellAndHeading(RobotExample example) =>
        AssertEndState(new WalkingRobotSimulationIISolution.RobotByPerimeterFormula(example.Width, example.Height), example);

    private static void AssertEndState(WalkingRobotSimulationIISolution.IRobot robot, RobotExample example)
    {
        foreach (var steps in example.Moves)
        {
            robot.Move(steps);
        }

        Assert.Equal((example.ExpectedX, example.ExpectedY), robot.GetPosition());
        Assert.Equal(example.ExpectedDirection, robot.GetDirection());
    }

    // One LeetCode example: the grid, the Move script run against it, and the cell and
    // heading the robot ends on. The two expected coordinates and the direction are
    // named at the row that states them, so `ExpectedX` and `ExpectedY` are not two
    // interchangeable `int` positions and the ending heading is not a bare `string`.
    public readonly record struct RobotExample(
        int Width,
        int Height,
        int[] Moves,
        int ExpectedX,
        int ExpectedY,
        string ExpectedDirection);
}
