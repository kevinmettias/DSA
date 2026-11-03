namespace DSAExperimentation.LeetCode.Solutions.RobotBoundedInCircle;

// The four facings LC 1041's robot can hold, in the order a right turn walks them:
// turning is one modular step along this sequence and wraps back to North, which is
// why the values are written down rather than left to the declaration order. North
// is 0, the facing a fresh robot starts with and the one a changed facing is
// measured against. The switch baseline's own copy of Domain/Devices/Compass's
// CompassHeading (ARCHITECTURE §17.5); the lookup-table arm uses that instead.
internal enum RobotDirection
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
}
