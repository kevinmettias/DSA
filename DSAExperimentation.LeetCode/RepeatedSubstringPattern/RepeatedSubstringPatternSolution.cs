using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RepeatedSubstringPattern;

// LeetCode 459. Repeated Substring Pattern: whether text is built from one or
// more repetitions of a smaller substring.
//
// HasRepeatedSubstringPatternByDivisorBruteForce tries every candidate period length
// that evenly divides n and verifies it by direct comparison (O(n^2) worst case) -
// the textbook baseline the composed strategy below is measured against.
// HasRepeatedSubstringPatternByKmpFailureFunction reuses this repo's own
// PrefixFunctionSearch.ComputeFailureFunction: text repeats exactly when its own longest
// proper-prefix-that-is-also-a-suffix, taken at the last position, is nonzero and
// evenly divides the string length - the same fact PrefixFunctionSearch's own doc
// comment calls out beyond FindAll's matching loop.
internal static class RepeatedSubstringPatternSolution
{
    private const int MaxPeriodDivisor = 2;

    public static bool HasRepeatedSubstringPatternByDivisorBruteForce(string text)
    {
        var n = text.Length;

        for (var period = 1; period <= n / MaxPeriodDivisor; period++)
        {
            if (n % period != 0)
            {
                continue;
            }

            if (IsRepeatedWithPeriod(text, period))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsRepeatedWithPeriod(string text, int period)
    {
        for (var i = period; i < text.Length; i++)
        {
            if (text[i] != text[i - period])
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasRepeatedSubstringPatternByKmpFailureFunction(string text)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(text);
        var longestBorder = failure[^1];
        var period = text.Length - longestBorder;

        return longestBorder != 0 && text.Length % period == 0;
    }
}
