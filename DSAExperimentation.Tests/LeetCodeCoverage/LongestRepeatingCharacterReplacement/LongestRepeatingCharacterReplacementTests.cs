using DSAExperimentation.LeetCode.LongestRepeatingCharacterReplacement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestRepeatingCharacterReplacement;

// LeetCode 424. Longest Repeating Character Replacement: harness only. Both
// strategies are LongestRepeatingCharacterReplacementSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class LongestRepeatingCharacterReplacementTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "ABAB", 2, 4 },
            { "AABABBA", 1, 4 },
            { "AAAA", 2, 4 },
            { "", 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRunByBruteForce_LeetCodeExamples_ReturnsLongestAchievableRun(
        string text, int maxReplacements, int expected)
    {
        var actual = LongestRepeatingCharacterReplacementSolution.LongestRunByBruteForce(text, maxReplacements);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRunBySlidingWindowHashMap_LeetCodeExamples_ReturnsLongestAchievableRun(
        string text, int maxReplacements, int expected)
    {
        var actual = LongestRepeatingCharacterReplacementSolution.LongestRunBySlidingWindowHashMap(text, maxReplacements);

        Assert.Equal(expected, actual);
    }
}
