using DSAExperimentation.LeetCode.RepeatedStringMatch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedStringMatch;

// Harness only. Both substring-search strategies live in RepeatedStringMatchSolution
// and are asserted against the same examples, including the case with no valid
// repeat count at all.
public sealed class RepeatedStringMatchTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "abcd", "cdabcdab", 3 },
            { "a", "aa", 2 },
            { "abc", "wxyz", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinRepeatsByStringContains_LeetCodeExamples_ReturnsMinimumRepeatCount(
        string a, string b, int expected) =>
        Assert.Equal(
            expected,
            RepeatedStringMatchSolution.MinRepeatsByStringContains(
                new RepeatedStringMatchSolution.RepeatedUnit(a),
                new RepeatedStringMatchSolution.TargetPattern(b)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinRepeatsByPrefixFunctionSearch_LeetCodeExamples_ReturnsMinimumRepeatCount(
        string a, string b, int expected) =>
        Assert.Equal(
            expected,
            RepeatedStringMatchSolution.MinRepeatsByPrefixFunctionSearch(
                new RepeatedStringMatchSolution.RepeatedUnit(a),
                new RepeatedStringMatchSolution.TargetPattern(b)));
}
