using DSAExperimentation.LeetCode.RepeatedStringMatch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedStringMatch;

// Harness only. Both substring-search strategies live in RepeatedStringMatchSolution
// and are asserted against the same examples, including the case with no valid
// repeat count at all.
public sealed partial class RepeatedStringMatchTests
{
    public static TheoryData<RepeatsExample> Examples =>
        new()
        {
            new RepeatsExample(Unit: "abcd", Target: "cdabcdab", Expected: 3),
            new RepeatsExample(Unit: "a", Target: "aa", Expected: 2),
            new RepeatsExample(Unit: "abc", Target: "wxyz", Expected: -1),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinRepeatsByStringContains_LeetCodeExamples_ReturnsMinimumRepeatCount(
        RepeatsExample example)
    {
        var actual = RepeatedStringMatchSolution.MinRepeatsByStringContains(
            new RepeatedStringMatchSolution.RepeatedUnit(example.Unit),
            new RepeatedStringMatchSolution.TargetPattern(example.Target));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinRepeatsByPrefixFunctionSearch_LeetCodeExamples_ReturnsMinimumRepeatCount(
        RepeatsExample example)
    {
        var actual = RepeatedStringMatchSolution.MinRepeatsByPrefixFunctionSearch(
            new RepeatedStringMatchSolution.RepeatedUnit(example.Unit),
            new RepeatedStringMatchSolution.TargetPattern(example.Target));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the unit being repeated, the target pattern it is
    // repeated to cover, and the repeat count that covers it (-1 when no count
    // does). The two strings are the solution's two positions, so the bundle names
    // each one where the row is written rather than leaving them adjacent and
    // swappable.
    public readonly record struct RepeatsExample(string Unit, string Target, int Expected);
}
