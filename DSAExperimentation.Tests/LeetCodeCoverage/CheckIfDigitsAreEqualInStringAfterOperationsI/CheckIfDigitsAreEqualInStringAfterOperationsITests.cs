using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDigitsAreEqualInStringAfterOperationsI;

// Harness only. Both the direct reduction and the Pascal-row closed form live in
// CheckIfDigitsAreEqualInStringAfterOperationsISolution - this file just pins both
// strategies to LeetCode's published examples.
public sealed class CheckIfDigitsAreEqualInStringAfterOperationsITests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "3902", true },
            { "34789", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreEqualByAdjacentSumReduction_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        string s, bool expected) =>
        Assert.Equal(
            expected, CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByAdjacentSumReduction(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreEqualByPascalRowCoefficients_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        string s, bool expected) =>
        Assert.Equal(
            expected, CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByPascalRowCoefficients(s));
}
