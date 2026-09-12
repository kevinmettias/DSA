using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RepeatedSubstringPattern;

// LeetCode 459. Repeated Substring Pattern: whether s is built from one or more
// repetitions of a smaller substring.
//
// HasRepeatedSubstringPatternByDivisorBruteForce tries every candidate period length
// that evenly divides n and verifies it by direct comparison (O(n^2) worst case) -
// the textbook baseline the composed strategy below is measured against.
// HasRepeatedSubstringPatternByKmpFailureFunction reuses this repo's own
// PrefixFunctionSearch.ComputeFailureFunction: s repeats exactly when its own longest
// proper-prefix-that-is-also-a-suffix, taken at the last position, is nonzero and
// evenly divides the string length - the same fact PrefixFunctionSearch's own doc
// comment calls out beyond FindAll's matching loop.
internal static class RepeatedSubstringPatternSolution
{
    private const int MaxPeriodDivisor = 2;

    public static bool HasRepeatedSubstringPatternByDivisorBruteForce(string s)
    {
        var n = s.Length;

        for (var period = 1; period <= n / MaxPeriodDivisor; period++)
        {
            if (n % period != 0)
            {
                continue;
            }

            if (RepeatsWithPeriod(s, period))
            {
                return true;
            }
        }

        return false;
    }

    private static bool RepeatsWithPeriod(string s, int period)
    {
        for (var i = period; i < s.Length; i++)
        {
            if (s[i] != s[i - period])
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasRepeatedSubstringPatternByKmpFailureFunction(string s)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(s);
        var longestBorder = failure[^1];
        var period = s.Length - longestBorder;

        return longestBorder != 0 && s.Length % period == 0;
    }
}
