using DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToRevertWordToInitialStateI;

// Harness only. Both strategies live in
// MinimumTimeToRevertWordToInitialStateISolution - this file just pins them
// to LeetCode's published examples.
public sealed class MinimumTimeToRevertWordToInitialStateITests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "abacaba", 3, 2 },
            { "abacaba", 4, 1 },
            { "abcbabcd", 2, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByBruteForce_LeetCodeExamples_ReturnsMinimumSeconds(string word, int k, int expected)
    {
        var actual = MinimumTimeToRevertWordToInitialStateISolution.MinTimeByBruteForce(word, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByZFunction_LeetCodeExamples_ReturnsMinimumSeconds(string word, int k, int expected)
    {
        var actual = MinimumTimeToRevertWordToInitialStateISolution.MinTimeByZFunction(word, k);

        Assert.Equal(expected, actual);
    }
}
