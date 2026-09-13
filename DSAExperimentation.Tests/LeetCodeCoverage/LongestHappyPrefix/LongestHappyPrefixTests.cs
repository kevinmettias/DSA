using DSAExperimentation.LeetCode.LongestHappyPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestHappyPrefix;

// Harness only. Both strategies are LongestHappyPrefixSolution's - this file just
// pins them to LeetCode's published examples, one theory per strategy so a failure
// names the strategy that broke.
public sealed class LongestHappyPrefixTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            // LeetCode example 1: "l" is the only prefix that is also a suffix.
            { "level", "l" },

            // LeetCode example 2: the whole 4-character run repeats at the end.
            { "leetcodeleet", "leet" },

            // Overlapping repeats: "abab" is both a prefix and a suffix of "ababab".
            { "ababab", "abab" },

            // No prefix is also a suffix.
            { "asdf", string.Empty },

            // A one-character string has no PROPER prefix that is also a suffix.
            { "a", string.Empty },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixByShrinkAndCompare_LeetCodeExamples_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        string s, string expected) =>
        Assert.Equal(expected, LongestHappyPrefixSolution.LongestPrefixByShrinkAndCompare(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixByPrefixFunction_LeetCodeExamples_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        string s, string expected) =>
        Assert.Equal(expected, LongestHappyPrefixSolution.LongestPrefixByPrefixFunction(s));
}
