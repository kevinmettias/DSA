using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedSubstringPattern;

// LeetCode 459. Repeated Substring Pattern: s is built from repetitions of a smaller
// substring exactly when its own KMP prefix function's longest proper-prefix-that-is-
// also-a-suffix, taken at the last position, both exists and evenly divides the string
// length - reusing this repo's own PrefixFunctionSearch.ComputeFailureFunction, whose
// own doc comment already calls out this exact kind of use beyond FindAll's matching
// loop.
public sealed partial class RepeatedSubstringPatternTests
{
    [Theory]
    [InlineData("abab", true)]
    [InlineData("aba", false)]
    [InlineData("abcabcabcabc", true)]
    [InlineData("a", false)]
    [InlineData("aa", true)]
    public void HasRepeatedSubstringPattern_ClassicExamples_ReturnsExpected(string s, bool expected)
        => Assert.Equal(expected, HasRepeatedSubstringPattern(s));

    private static bool HasRepeatedSubstringPattern(string s)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(s);
        var longestBorder = failure[^1];
        var period = s.Length - longestBorder;

        return longestBorder != 0 && s.Length % period == 0;
    }
}
