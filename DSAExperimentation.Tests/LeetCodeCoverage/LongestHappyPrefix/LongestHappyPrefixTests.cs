using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestHappyPrefix;

// LeetCode 1392. Longest Happy Prefix: a "happy prefix" is exactly what KMP's own
// prefix/failure function measures - the longest proper prefix of s that is also
// a suffix of s. Running PrefixFunctionSearch.ComputeFailureFunction over s
// against itself and reading its last entry gives that length directly, with no
// separate algorithm needed.
public sealed partial class LongestHappyPrefixTests
{
    [Fact]
    public void LongestPrefix_ClassicExample_ReturnsSingleRepeatingCharacter()
    {
        var prefix = LongestPrefix("level");

        Assert.Equal("l", prefix);
    }

    [Fact]
    public void LongestPrefix_OverlappingRepeatedRun_ReturnsLongestMatch()
    {
        var prefix = LongestPrefix("leetcodeleet");

        Assert.Equal("leet", prefix);
    }

    [Fact]
    public void LongestPrefix_NoPrefixIsAlsoASuffix_ReturnsEmptyString()
    {
        var prefix = LongestPrefix("asdf");

        Assert.Equal(string.Empty, prefix);
    }

    private static string LongestPrefix(string s)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(s);
        var length = failure.Length == 0 ? 0 : failure[^1];
        return s[..length];
    }
}
