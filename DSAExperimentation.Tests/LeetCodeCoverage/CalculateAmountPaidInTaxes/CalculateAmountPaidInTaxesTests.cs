using DSAExperimentation.LeetCode.CalculateAmountPaidInTaxes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CalculateAmountPaidInTaxes;

// Harness only. Both the raw jagged-array walk and the ArraySequence<T> walk are
// CalculateAmountPaidInTaxesSolution's - this file pins them to LeetCode's published
// examples plus the zero-income, single-bracket and income-above-the-top-bracket
// cases, where the loop either never taxes anything or never gets to break early.
public sealed class CalculateAmountPaidInTaxesTests
{
    private const int Precision = 5;

    public static TheoryData<int[][], int, double> Examples =>
        new()
        {
            { [[3, 50], [7, 10], [12, 25]], 10, 2.65 },
            { [[1, 0], [4, 25], [5, 50]], 2, 0.25 },
            { [[1, 0], [4, 25], [5, 50]], 0, 0.0 },
            { [[2, 50], [4, 10], [10, 25]], 0, 0.0 },
            { [[2, 50], [6, 10], [8, 25]], 8, 1.9 },
            { [[10, 100]], 5, 5.0 },
            { [[3, 50], [7, 10]], 100, 1.9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalculateTaxByBracketArrayWalk_LeetCodeExamples_ReturnsExpectedAmount(
        int[][] brackets, int income, double expected) =>
        Assert.Equal(
            expected,
            CalculateAmountPaidInTaxesSolution.CalculateTaxByBracketArrayWalk(brackets, income),
            Precision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalculateTaxByRandomAccessSequence_LeetCodeExamples_ReturnsExpectedAmount(
        int[][] brackets, int income, double expected) =>
        Assert.Equal(
            expected,
            CalculateAmountPaidInTaxesSolution.CalculateTaxByRandomAccessSequence(brackets, income),
            Precision);
}
