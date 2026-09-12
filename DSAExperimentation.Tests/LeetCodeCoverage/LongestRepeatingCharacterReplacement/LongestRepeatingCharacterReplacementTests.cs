using DSAExperimentation.LeetCode.LongestRepeatingCharacterReplacement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestRepeatingCharacterReplacement;

// LeetCode 424. Longest Repeating Character Replacement: harness only. Both
// strategies are LongestRepeatingCharacterReplacementSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class LongestRepeatingCharacterReplacementTests
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
        string s, int k, int expected) =>
        Assert.Equal(expected, LongestRepeatingCharacterReplacementSolution.LongestRunByBruteForce(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRunBySlidingWindowHashMap_LeetCodeExamples_ReturnsLongestAchievableRun(
        string s, int k, int expected) =>
        Assert.Equal(expected, LongestRepeatingCharacterReplacementSolution.LongestRunBySlidingWindowHashMap(s, k));
}
