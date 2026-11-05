using DSAExperimentation.LeetCode.Solutions.CalculateAmountPaidInTaxes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CalculateAmountPaidInTaxes;

// Harness only. Both the raw jagged-array walk and the ArraySequence<T> walk are
// CalculateAmountPaidInTaxesSolution's - this file pins them to the zero-income,
// single-bracket and income-at-or-above-the-top-bracket cases, where the loop either
// never taxes anything or never gets to break early. The bracket sequence the second
// walk is handed is asserted on its own. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class CalculateAmountPaidInTaxesSolutionTests
{
    private const int Precision = 5;

    public static TheoryData<int[][], int, double> Cases =>
        new()
        {
            { [[1, 0], [4, 25], [5, 50]], 0, 0.0 },
            { [[2, 50], [4, 10], [10, 25]], 0, 0.0 },
            { [[2, 50], [6, 10], [8, 25]], 8, 1.9 },
            { [[10, 100]], 5, 5.0 },
            { [[3, 50], [7, 10]], 100, 1.9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalculateTaxByBracketArrayWalk_Cases_ReturnsExpectedAmount(
        int[][] brackets, int income, double expected)
    {
        var taxDue = CalculateAmountPaidInTaxesSolution.CalculateTaxByBracketArrayWalk(brackets, income);

        Assert.Equal(expected, taxDue, Precision);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalculateTaxByRandomAccessSequence_Cases_ReturnsExpectedAmount(
        int[][] brackets, int income, double expected)
    {
        var taxDue =
            CalculateAmountPaidInTaxesSolution.CalculateTaxByRandomAccessSequence(brackets, income);

        Assert.Equal(expected, taxDue, Precision);
    }

    // Each [upper, percent] row becomes one (Upper, Percent) pair, in the order given:
    // [3, 50] -> (3, 50), [7, 10] -> (7, 10) and [12, 25] -> (12, 25).
    [Fact]
    public void ToBracketSequence_LeetCodeFirstExample_PairsEachUpperBoundWithItsPercent()
    {
        var brackets = CalculateAmountPaidInTaxesSolution.ToBracketSequence([[3, 50], [7, 10], [12, 25]]);

        Assert.Equal([(3, 50), (7, 10), (12, 25)], Enumerable.Range(0, brackets.Length).Select(brackets.Get));
    }
}
