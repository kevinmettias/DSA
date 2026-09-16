using DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfValidStringsToFormTargetI;

// Harness only. Both reach-computation strategies feed the same Jump-Game-II sweep
// inside MinimumNumberOfValidStringsToFormTargetISolution - this file just pins
// them to LeetCode's published examples, including the unreachable case.
public sealed partial class MinimumNumberOfValidStringsToFormTargetITests
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
        var actual = MinimumNumberOfValidStringsToFormTargetISolution.MinValidStringsByBruteForce(words, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinValidStringsByZFunctionAcrossWords_LeetCodeExamples_ReturnsFewestValidStrings(
        string[] words, string target, int expected)
    {
        var actual = MinimumNumberOfValidStringsToFormTargetISolution.MinValidStringsByZFunctionAcrossWords(words, target);
        Assert.Equal(expected, actual);
    }
}
