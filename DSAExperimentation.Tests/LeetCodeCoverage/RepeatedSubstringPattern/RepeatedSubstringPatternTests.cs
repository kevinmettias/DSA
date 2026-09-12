using DSAExperimentation.LeetCode.RepeatedSubstringPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedSubstringPattern;

// Harness only. Both strategies are RepeatedSubstringPatternSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class RepeatedSubstringPatternTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "abab", true },
            { "aba", false },
            { "abcabcabcabc", true },
            { "a", false },
            { "aa", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasRepeatedSubstringPatternByDivisorBruteForce_ClassicExamples_ReturnsExpected(
        string s, bool expected) =>
        Assert.Equal(expected, RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByDivisorBruteForce(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasRepeatedSubstringPatternByKmpFailureFunction_ClassicExamples_ReturnsExpected(
        string s, bool expected) =>
        Assert.Equal(expected, RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByKmpFailureFunction(s));
}
