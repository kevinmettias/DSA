namespace DSAExperimentation.LeetCode.RobotBoundedInCircle;

// The turn arithmetic LC 1041's robot is described in: the two turn sizes, and how
// many facings there are for RobotState.Turn to wrap at. The facings themselves are
// RobotDirection and the symbols an instruction string is made of are
// RobotInstruction; what stays here is the arithmetic that turns one facing into the
// next, which neither set can state about itself.
//
// Owned here rather than in RobotBoundedInCircleSolution because that class's two
// strategies are its own subject - these describe the robot, which both strategies
// only read.
internal static class RobotMotion
{
    public const int DirectionCount = 4;

    // Turning left is turning right three times, which keeps both turns inside the
    // same modular step and avoids a negative remainder.
    public const int LeftTurnOffset = 3;
    public const int RightTurnOffset = 1;
}
