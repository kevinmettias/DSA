using DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfValidStringsToFormTargetII;

// Harness only. Both reach-computation strategies feed the same Jump-Game-II sweep
// inside MinimumNumberOfValidStringsToFormTargetIISolution - this file just pins
// them to LeetCode's published examples, including the unreachable case.
public sealed class MinimumNumberOfValidStringsToFormTargetIITests
{
    public static TheoryData<string[], string, int> Examples =>
        new()
        {
            { ["abc", "aaaaa", "bcdef"], "aabcdabc", 3 },
            { ["abababab", "ab"], "ababaababa", 2 },
            { ["abcdef"], "xyz", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinValidStringsByBruteForce_LeetCodeExamples_ReturnsFewestValidStrings(
        string[] words, string target, int expected)
    {
        var actual = MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByBruteForce(words, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinValidStringsByZFunctionAcrossWords_LeetCodeExamples_ReturnsFewestValidStrings(
        string[] words, string target, int expected)
    {
        var actual = MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByZFunctionAcrossWords(words, target);
        Assert.Equal(expected, actual);
    }
}
