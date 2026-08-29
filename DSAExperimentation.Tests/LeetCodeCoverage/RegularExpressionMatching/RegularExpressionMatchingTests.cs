using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RegularExpressionMatching;

// LeetCode 10. Regular Expression Matching: the recurrence is the textbook
// (inputIndex, patternIndex) DP, with this repo's Memoizer supplying the cache.
public sealed partial class RegularExpressionMatchingTests
{
    [Theory]
    [InlineData("aa", "a", false)]
    [InlineData("aa", "a*", true)]
    [InlineData("ab", ".*", true)]
    [InlineData("aab", "c*a*b", true)]
    [InlineData("mississippi", "mis*is*p*.", false)]
    [InlineData("", "c*", true)]
    public void IsMatch_LeetCodeExamples_ReturnsExpectedResult(string text, string pattern, bool expected)
        => Assert.Equal(expected, IsMatch(text, pattern));

    private static bool IsMatch(string text, string pattern)
    {
        return Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom);

        bool MatchFrom(
            (int Text, int Pattern) state,
            Func<(int Text, int Pattern), bool> match)
        {
            var (textIndex, patternIndex) = state;

            if (patternIndex == pattern.Length)
            {
                return textIndex == text.Length;
            }

            var firstMatches = textIndex < text.Length
                && (pattern[patternIndex] == text[textIndex] || pattern[patternIndex] == '.');

            if (patternIndex + 1 < pattern.Length && pattern[patternIndex + 1] == '*')
            {
                return match((textIndex, patternIndex + 2))
                    || (firstMatches && match((textIndex + 1, patternIndex)));
            }

            return firstMatches && match((textIndex + 1, patternIndex + 1));
        }
    }
}
