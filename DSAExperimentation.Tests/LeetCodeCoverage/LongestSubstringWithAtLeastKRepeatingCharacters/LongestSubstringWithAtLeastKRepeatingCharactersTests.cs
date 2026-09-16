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
    public void LongestByBruteForce_LeetCodeExamples_ReturnsLongestQualifyingLength(
        string text, int minimumRepeats, int expected)
    {
        var actual = LongestSubstringWithAtLeastKRepeatingCharactersSolution
            .LongestByBruteForce(text, minimumRepeats);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestByDivideAndConquer_LeetCodeExamples_ReturnsLongestQualifyingLength(
        string text, int minimumRepeats, int expected)
    {
        var actual = LongestSubstringWithAtLeastKRepeatingCharactersSolution
            .LongestByDivideAndConquer(text, minimumRepeats);
        Assert.Equal(expected, actual);
    }
}
