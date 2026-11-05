using DSAExperimentation.LeetCode.Solutions.WalkingRobotSimulationII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WalkingRobotSimulationII;

// On LeetCode's 6 by 3 grid, where the robot starts at (0, 0) facing East.
public sealed partial class WalkingRobotSimulationIICallsTests
{
    private const string East = "East";

    [Fact]
    public void Step_TwoCells_MovesTheRobotAlongItsHeading()
    {
        var robot = WalkingRobotSimulationIISolution.CreateByStepSimulation(6, 3);

        WalkingRobotSimulationIICalls.Step(robot, 2);

        Assert.Equal((2, 0), robot.GetPosition());
    }

    [Fact]
    public void GetPos_NewRobot_IsTheOrigin() =>
        Assert.Equal((0, 0), WalkingRobotSimulationIICalls.GetPos(WalkingRobotSimulationIISolution.CreateByStepSimulation(6, 3)));

    [Fact]
    public void GetDir_NewRobot_IsEast() =>
        Assert.Equal(East, WalkingRobotSimulationIICalls.GetDir(WalkingRobotSimulationIISolution.CreateByStepSimulation(6, 3)));
}
