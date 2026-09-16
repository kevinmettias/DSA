using DSAExperimentation.LeetCode.RepeatedSubstringPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedSubstringPattern;

// Harness only. Both strategies are RepeatedSubstringPatternSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class RepeatedSubstringPatternTests
{
    public static TheoryData<RepeatedPatternExample> Examples =>
        new()
        {
            new RepeatedPatternExample(Input: "abab", HasRepeatedPattern: true),
            new RepeatedPatternExample(Input: "aba", HasRepeatedPattern: false),
            new RepeatedPatternExample(Input: "abcabcabcabc", HasRepeatedPattern: true),
            new RepeatedPatternExample(Input: "a", HasRepeatedPattern: false),
            new RepeatedPatternExample(Input: "aa", HasRepeatedPattern: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasRepeatedSubstringPatternByDivisorBruteForce_ClassicExamples_ReturnsExpected(
        RepeatedPatternExample example) =>
        Assert.Equal(
            example.HasRepeatedPattern,
            RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByDivisorBruteForce(example.Input));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasRepeatedSubstringPatternByKmpFailureFunction_ClassicExamples_ReturnsExpected(
        RepeatedPatternExample example) =>
        Assert.Equal(
            example.HasRepeatedPattern,
            RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByKmpFailureFunction(example.Input));

    // One example: the string and whether it is a substring repeated twice or more.
    // The expectation is named rather than carried by its position, so the row reads
    // as an assertion instead of as a bare `true`.
    public readonly record struct RepeatedPatternExample(string Input, bool HasRepeatedPattern);
}
