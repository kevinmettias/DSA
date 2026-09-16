using DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonPrefixOfKStringsAfterRemoval;

// Harness only. Both strategies are
// LongestCommonPrefixOfKStringsAfterRemovalSolution's - this file just pins them
// to LeetCode's published examples, including the duplicate-heavy first example
// that exercises per-word multiplicity rather than mere prefix presence.
public sealed partial class LongestCommonPrefixOfKStringsAfterRemovalTests
{
    public static TheoryData<string[], int, int[]> Examples =>
        new()
        {
            { ["jump", "run", "run", "jump", "run"], 2, [3, 4, 4, 3, 4] },
            { ["dog", "racer", "car"], 2, [0, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByBruteForce_LeetCodeExamples_ReturnsLongestSharedPrefixPerRemoval(
        string[] words, int requiredShareCount, int[] expected)
    {
        var actual = LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByBruteForce(words, requiredShareCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByReduceTrie_LeetCodeExamples_ReturnsLongestSharedPrefixPerRemoval(
        string[] words, int requiredShareCount, int[] expected)
    {
        var actual = LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByReduceTrie(words, requiredShareCount);

        Assert.Equal(expected, actual);
    }
}
