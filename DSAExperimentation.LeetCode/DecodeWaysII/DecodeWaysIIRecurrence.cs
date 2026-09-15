namespace DSAExperimentation.LeetCode.DecodeWaysII;

// The values LC 639's decode recurrence is written in terms of: how many codes each
// wildcard shape stands for, plus the two arithmetic facts the digit pair needs (the
// base a pair is read in, and the largest code that still names a letter).
//
// Owned here rather than in DecodeWaysIISolution because there are nine of them and
// that class's recurrence is its own subject: somebody tuning a wildcard count should
// not have to step over the recurrence to find it, and somebody reading the recurrence
// should not have to step over the counts first.
internal static class DecodeWaysIIRecurrence
{
    // Every running total the recurrence produces is reduced mod this, as LeetCode
    // requires.
    public const long Mod = 1_000_000_007;

    // A pair is two characters, read in base ten.
    public const int PairLength = 2;
    public const int DecimalBase = 10;

    // The largest two-digit code that still names a letter (Z is 26).
    public const int MaxLetterCode = 26;

    // How many codes each wildcard shape stands for: '*' alone is any digit 1-9,
    // "**" is the 15 valid pairs, "*x" is 2 while x <= '6', and "*x" behind a
    // leading '1' or '2' is the 9 or 6 pairs those digits can still complete.
    public const int SingleWildcardWays = 9;
    public const int BothWildcardPairWays = 15;
    public const int StarThenSmallDigitWays = 2;
    public const int FirstIsOneStarWays = 9;
    public const int FirstIsTwoStarWays = 6;
}
