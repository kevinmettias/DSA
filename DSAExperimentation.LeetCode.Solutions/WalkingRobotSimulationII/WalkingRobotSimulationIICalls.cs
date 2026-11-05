namespace DSAExperimentation.LeetCode.Solutions.WalkingRobotSimulationII;

// LeetCode's three calls on the robot, which IRobot answers under its own names (ARCHITECTURE 17.13): step is
// Move, getPos is GetPosition, and getDir is GetDirection.
internal static class WalkingRobotSimulationIICalls
{
    public static void Step(WalkingRobotSimulationIISolution.IRobot robot, int num) => robot.Move(num);

    public static (int X, int Y) GetPos(WalkingRobotSimulationIISolution.IRobot robot) => robot.GetPosition();

    public static string GetDir(WalkingRobotSimulationIISolution.IRobot robot) => robot.GetDirection();
}
