using DSAExperimentation.LeetCode.LongestSubstringWithAtLeastKRepeatingCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringWithAtLeastKRepeatingCharacters;

// Harness only. Both strategies are
// LongestSubstringWithAtLeastKRepeatingCharactersSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class LongestSubstringWithAtLeastKRepeatingCharactersTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "aaabb", 3, 3 },
            { "ababbc", 2, 5 },
            { "weitong", 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestByBruteForce_LeetCodeExamples_ReturnsLongestQualifyingLength(string s, int k, int expected) =>
        Assert.Equal(expected, LongestSubstringWithAtLeastKRepeatingCharactersSolution.LongestByBruteForce(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestByDivideAndConquer_LeetCodeExamples_ReturnsLongestQualifyingLength(string s, int k, int expected) =>
        Assert.Equal(expected, LongestSubstringWithAtLeastKRepeatingCharactersSolution.LongestByDivideAndConquer(s, k));
}
