using DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToRevertWordToInitialStateII;

// Harness only. Both strategies live in
// MinimumTimeToRevertWordToInitialStateIISolution - this file just pins them
// to LeetCode's published examples (identical to 3029's, since 3031 restates
// the same problem at a larger bound).
public sealed class MinimumTimeToRevertWordToInitialStateIITests
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
        var actual = MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByBruteForce(word, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByZFunction_LeetCodeExamples_ReturnsMinimumSeconds(string word, int k, int expected)
    {
        var actual = MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByZFunction(word, k);

        Assert.Equal(expected, actual);
    }
}
