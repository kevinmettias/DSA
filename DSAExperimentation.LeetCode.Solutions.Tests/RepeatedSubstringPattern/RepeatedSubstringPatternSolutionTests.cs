using DSAExperimentation.LeetCode.Solutions.RepeatedSubstringPattern;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RepeatedSubstringPattern;

// Harness only. Both strategies are RepeatedSubstringPatternSolution's - this file
// just pins them to the shortest repeated string, a single letter written twice.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RepeatedSubstringPatternSolutionTests
{
    public static TheoryData<RepeatedPatternCase> Cases =>
        new()
        {
            new RepeatedPatternCase(Input: "aa", HasRepeatedPattern: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasRepeatedSubstringPatternByDivisorBruteForce_Cases_ReturnsExpected(
        RepeatedPatternCase testCase) =>
        Assert.Equal(
            testCase.HasRepeatedPattern,
            RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByDivisorBruteForce(testCase.Input));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasRepeatedSubstringPatternByKmpFailureFunction_Cases_ReturnsExpected(
        RepeatedPatternCase testCase) =>
        Assert.Equal(
            testCase.HasRepeatedPattern,
            RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByKmpFailureFunction(testCase.Input));

    // One case: the string and whether it is a substring repeated twice or more.
    // The expectation is named rather than carried by its position, so the row reads
    // as an assertion instead of as a bare `true`.
    public readonly record struct RepeatedPatternCase(string Input, bool HasRepeatedPattern);
}
