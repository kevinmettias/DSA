using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDigitsAreEqualInStringAfterOperationsI;

// Harness only. Both the direct reduction and the Pascal-row closed form live in
// CheckIfDigitsAreEqualInStringAfterOperationsISolution - this file just pins both
// strategies to LeetCode's published examples.
public sealed class CheckIfDigitsAreEqualInStringAfterOperationsITests
{
    public static TheoryData<DigitsMatchExample> Examples =>
        new()
        {
            { new DigitsMatchExample(S: "3902", Expected: true) },
            { new DigitsMatchExample(S: "34789", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreEqualByAdjacentSumReduction_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByAdjacentSumReduction(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreEqualByPascalRowCoefficients_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByPascalRowCoefficients(example.S));

    // One LeetCode example: the digit string and whether the two final digits match. The
    // row names both positions - a bare `bool` argument would read as "true" and say
    // nothing about what is true.
    public readonly record struct DigitsMatchExample(string S, bool Expected);
}
