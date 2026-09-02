using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDigitsAreEqualInStringAfterOperationsII;

// Harness only. Both reduction strategies are
// CheckIfDigitsAreEqualInStringAfterOperationsIISolution's - this file just pins
// them to LeetCode's published examples.
public sealed class CheckIfDigitsAreEqualInStringAfterOperationsIITests
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
        Assert.Equal(expected, CheckIfDigitsAreEqualInStringAfterOperationsIISolution.AreEqualByAdjacentSumReduction(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreEqualByLucasBinomialCoefficients_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        string s, bool expected) =>
        Assert.Equal(expected, CheckIfDigitsAreEqualInStringAfterOperationsIISolution.AreEqualByLucasBinomialCoefficients(s));
}
