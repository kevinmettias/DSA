namespace DSAExperimentation.LeetCode.RobotBoundedInCircle;

// The three symbols an LC 1041 instruction string is made of, valued at the
// characters themselves: the alphabet belongs to the string and the alternatives the
// program routes between belong to the domain, so one declaration says both, and
// reading a symbol is a cast rather than a second table that has to be kept in step
// with this one. A fourth instruction is a compile error at every switch that routes
// one.
internal enum RobotInstruction
{
    // 'G': step one unit along the current facing.
    Forward = 'G',

    // 'L': turn a quarter turn anticlockwise.
    Left = 'L',

    // 'R': turn a quarter turn clockwise.
    Right = 'R',
}
