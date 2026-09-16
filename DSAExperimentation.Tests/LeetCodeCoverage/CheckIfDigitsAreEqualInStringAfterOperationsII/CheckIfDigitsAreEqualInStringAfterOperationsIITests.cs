using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDigitsAreEqualInStringAfterOperationsII;

// Harness only. Both reduction strategies are
// CheckIfDigitsAreEqualInStringAfterOperationsIISolution's - this file just pins
// them to LeetCode's published examples.
public sealed class CheckIfDigitsAreEqualInStringAfterOperationsIITests
{
    public static TheoryData<DigitsMatchExample> Examples =>
        new()
        {
            { new DigitsMatchExample(S: "3902", Expected: true) },
            { new DigitsMatchExample(S: "34789", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsEqualByAdjacentSumReduction_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsIISolution.IsEqualByAdjacentSumReduction(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsEqualByLucasBinomialCoefficients_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsIISolution.IsEqualByLucasBinomialCoefficients(example.S));

    // One LeetCode example: the digit string and whether the two final digits match. The
    // row names both positions - a bare `bool` argument would read as "true" and say
    // nothing about what is true.
    public readonly record struct DigitsMatchExample(string S, bool Expected);
}
