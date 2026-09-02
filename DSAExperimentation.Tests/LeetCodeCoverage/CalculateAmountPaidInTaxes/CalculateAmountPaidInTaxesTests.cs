using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CalculateAmountPaidInTaxes;

// LeetCode 2303. Calculate Amount Paid in Taxes: a single forward pass over the
// already-sorted tax brackets, taxing only the slice of income that falls inside
// each bracket. The brackets are wrapped in this repo's own ArraySequence<T> - the
// O(1)-Get witness of IRandomAccessSequence<T> (ARCHITECTURE.md §9) - since "an
// already-sorted, indexable sequence" is exactly what that Representation contract
// models, the same shape BinarySearch's own sequence argument already uses.
public sealed partial class CalculateAmountPaidInTaxesTests
{
    [Theory]
    [InlineData(new int[] { 3, 50, 7, 10, 12, 25 }, 10, 2.65)]
    [InlineData(new int[] { 1, 0, 4, 25, 5, 50 }, 0, 0.0)]
    [InlineData(new int[] { 2, 50, 6, 10, 8, 25 }, 8, 1.9)]
    public void CalculateTax_LeetCodeExamples_ReturnsExpectedAmount(int[] flatBrackets, int income, double expected)
    {
        var brackets = ToBracketSequence(flatBrackets);

        var tax = CalculateTax(brackets, income);

        Assert.Equal(expected, tax, precision: 5);
    }

    private static double CalculateTax(ArraySequence<(int Upper, int Percent)> brackets, int income)
    {
        var tax = 0.0;
        var previousUpper = 0;

        for (var i = 0; i < brackets.Length; i++)
        {
            var (upper, percent) = brackets.Get(i);
            var taxableInBracket = Math.Max(0, Math.Min(income, upper) - previousUpper);
            tax += taxableInBracket * percent / 100.0;
            previousUpper = upper;

            if (income <= upper)
            {
                break;
            }
        }

        return tax;
    }

    private static ArraySequence<(int Upper, int Percent)> ToBracketSequence(int[] flatBrackets)
    {
        var pairs = new (int Upper, int Percent)[flatBrackets.Length / 2];

        for (var i = 0; i < pairs.Length; i++)
        {
            pairs[i] = (flatBrackets[i * 2], flatBrackets[i * 2 + 1]);
        }

        return new ArraySequence<(int Upper, int Percent)>(pairs);
    }
}
