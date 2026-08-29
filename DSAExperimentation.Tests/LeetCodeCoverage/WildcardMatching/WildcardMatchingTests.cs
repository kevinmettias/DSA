using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WildcardMatching;

public sealed partial class WildcardMatchingTests
{
    [Theory]
    [InlineData("aa", "a", false)]
    [InlineData("aa", "*", true)]
    [InlineData("cb", "?a", false)]
    [InlineData("adceb", "*a*b", true)]
    public void IsMatch_LeetCodeExamples_ReturnsExpected(string text, string pattern, bool expected)
        => Assert.Equal(expected, IsMatch(text, pattern));

    private static bool IsMatch(string text, string pattern)
    {
        return Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom);

        bool MatchFrom((int Text, int Pattern) state, Func<(int Text, int Pattern), bool> match)
        {
            var (i, j) = state;
            if (j == pattern.Length)
            {
                return i == text.Length;
            }

            if (pattern[j] == '*')
            {
                return match((i, j + 1)) || (i < text.Length && match((i + 1, j)));
            }

            return i < text.Length && (pattern[j] == '?' || pattern[j] == text[i]) && match((i + 1, j + 1));
        }
    }
}
