using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CalculateAmountPaidInTaxes;

// LeetCode 2303. Calculate Amount Paid in Taxes: the brackets arrive already sorted
// by upper bound, so the answer is a single forward pass that taxes only the slice
// of income falling inside each bracket and stops at the bracket the income lands
// in.
//
// There is no asymptotically worse brute force to compare against here - one pass is
// already optimal - so the two strategies differ purely in the Representation they
// walk. The baseline indexes LeetCode's own int[][] directly, the way you would
// write it with nothing but the BCL; the composed strategy walks the same brackets
// through this repo's ArraySequence<T>, the O(1)-Get witness of
// IRandomAccessSequence<T> (ARCHITECTURE.md §9), which is exactly the "an
// already-sorted, indexable sequence" contract BinarySearch's own sequence argument
// takes. The measurement is therefore the "Representation swap should cost nothing"
// comparison DesignBrowserHistory makes for List<string> vs. DynamicArray<string>.
internal static class CalculateAmountPaidInTaxesSolution
{
    // brackets[i] is [upper_i, percent_i]; percentages are whole numbers out of 100.
    private const int UpperIndex = 0;
    private const int PercentIndex = 1;
    private const double PercentScale = 100.0;

    // The textbook arm: walk LeetCode's jagged array with a raw indexer and nothing
    // else. Deliberately free of this repo's primitives - it is what the composed
    // strategy below has to justify itself against.
    public static double CalculateTaxByBracketArrayWalk(int[][] brackets, int income)
    {
        var tax = 0.0;
        var previousUpper = 0;

        for (var i = 0; i < brackets.Length; i++)
        {
            var upper = brackets[i][UpperIndex];
            var taxableInBracket = Math.Max(0, Math.Min(income, upper) - previousUpper);
            tax += taxableInBracket * brackets[i][PercentIndex] / PercentScale;
            previousUpper = upper;

            if (income <= upper)
            {
                break;
            }
        }

        return tax;
    }

    // The same forward pass expressed against IRandomAccessSequence<T>'s Get, so the
    // brackets are a Representation the caller chose rather than a bare array the
    // loop happens to know the shape of.
    public static double CalculateTaxByRandomAccessSequence(int[][] brackets, int income) =>
        CalculateTaxByRandomAccessSequence(ToBracketSequence(brackets), income);

    public static double CalculateTaxByRandomAccessSequence(
        ArraySequence<(int Upper, int Percent)> brackets, int income)
    {
        var tax = 0.0;
        var previousUpper = 0;

        for (var i = 0; i < brackets.Length; i++)
        {
            var (upper, percent) = brackets.Get(i);
            var taxableInBracket = Math.Max(0, Math.Min(income, upper) - previousUpper);
            tax += taxableInBracket * percent / PercentScale;
            previousUpper = upper;

            if (income <= upper)
            {
                break;
            }
        }

        return tax;
    }

    // Public so a benchmark can charge the [upper, percent] pairing to [GlobalSetup]
    // rather than to the pass being measured (ARCHITECTURE.md §17.4).
    public static ArraySequence<(int Upper, int Percent)> ToBracketSequence(int[][] brackets)
    {
        var pairs = new (int Upper, int Percent)[brackets.Length];

        for (var i = 0; i < pairs.Length; i++)
        {
            pairs[i] = (brackets[i][UpperIndex], brackets[i][PercentIndex]);
        }

        return new ArraySequence<(int Upper, int Percent)>(pairs);
    }
}
