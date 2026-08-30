using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorOfStrings;

// LeetCode 1071. Greatest Common Divisor of Strings: str1 and str2 share a common
// "divisor" string exactly when str1+str2 forms one repeating block - the same
// repeating-period question RepeatedSubstringPatternTests already answers via this
// repo's own PrefixFunctionSearch.ComputeFailureFunction, just applied to the
// concatenation str1+str2 instead of a single string. The failure function's last
// entry gives str1+str2's shortest period, and that period is the answer exactly
// when it evenly divides both original lengths - equivalent to (but never building)
// the textbook str1+str2==str2+str1 check; see GreatestCommonDivisorOfStringsBenchmarks
// for that comparison.
public sealed partial class GreatestCommonDivisorOfStringsTests
{
    [Fact]
    public void GcdOfStrings_ClassicExample_ReturnsSharedDivisor()
        => Assert.Equal("ABC", GcdOfStrings("ABCABC", "ABC"));

    [Fact]
    public void GcdOfStrings_SecondExample_ReturnsSharedDivisor()
        => Assert.Equal("AB", GcdOfStrings("ABABAB", "ABAB"));

    [Fact]
    public void GcdOfStrings_NoCommonDivisor_ReturnsEmptyString()
        => Assert.Equal(string.Empty, GcdOfStrings("LEET", "CODE"));

    private static string GcdOfStrings(string str1, string str2)
    {
        var concatenated = str1 + str2;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(concatenated);
        var period = concatenated.Length - failure[^1];

        if (str1.Length % period != 0 || str2.Length % period != 0)
        {
            return string.Empty;
        }

        return str1[..period];
    }
}
