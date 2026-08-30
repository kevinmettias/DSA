using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPalindrome;

// LeetCode 214. Shortest Palindrome: build s + separator + reverse(s) and reuse this repo's
// own KMP prefix/failure function (PrefixFunctionSearch.ComputeFailureFunction) - its value at
// the last index is exactly the length of the longest run that is simultaneously a prefix of s
// and a suffix of reverse(s), i.e. the longest palindromic PREFIX of s. Prepending the reverse
// of whatever's left over makes the whole string a palindrome with the fewest possible
// characters.
public sealed partial class ShortestPalindromeTests
{
    [Theory]
    [InlineData("aacecaaa", "aaacecaaa")]
    [InlineData("abcd", "dcbabcd")]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("aa", "aa")]
    [InlineData("racecar", "racecar")]
    public void BuildShortestPalindrome_VariousInputs_PrependsFewestCharacters(string s, string expected)
    {
        Assert.Equal(expected, BuildShortestPalindrome(s));
    }

    private static string BuildShortestPalindrome(string s)
    {
        if (s.Length == 0)
        {
            return s;
        }

        var reversed = new string(s.Reverse().ToArray());
        var combined = s + "#" + reversed;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(combined);
        var longestPalindromicPrefix = failure[^1];

        return new string(s[longestPalindromicPrefix..].Reverse().ToArray()) + s;
    }
}
