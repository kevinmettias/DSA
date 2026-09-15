using DSAExperimentation.LeetCode.RegularExpressionMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RegularExpressionMatching;

// Harness only: both strategies live in RegularExpressionMatchingSolution and are
// asserted against the same examples.
public sealed class RegularExpressionMatchingTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "aa", "a", false },
            { "aa", "a*", true },
            { "ab", ".*", true },
            { "aab", "c*a*b", true },
            { "mississippi", "mis*is*p*.", false },
            { "", "c*", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByRecursion_LeetCodeExamples_ReturnsExpectedResult(string text, string pattern, bool expected) =>
        Assert.Equal(
            expected,
            RegularExpressionMatchingSolution.IsMatchByRecursion(
                new RegularExpressionMatchingSolution.SubjectText(text),
                new RegularExpressionMatchingSolution.RegexPattern(pattern)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByMemoization_LeetCodeExamples_ReturnsExpectedResult(string text, string pattern, bool expected) =>
        Assert.Equal(
            expected,
            RegularExpressionMatchingSolution.IsMatchByMemoization(
                new RegularExpressionMatchingSolution.SubjectText(text),
                new RegularExpressionMatchingSolution.RegexPattern(pattern)));
}
