using DSAExperimentation.LeetCode.Solutions.LongestSubstringWithAtLeastKRepeatingCharacters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestSubstringWithAtLeastKRepeatingCharacters;

// Harness only. Both strategies are
// LongestSubstringWithAtLeastKRepeatingCharactersSolution's - this file just pins
// them to a string whose letters each appear once, so no substring qualifies.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LongestSubstringWithAtLeastKRepeatingCharactersSolutionTests
{
    public static TheoryData<string, int, int> Cases =>
        new()
        {
            { "weitong", 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestByBruteForce_Cases_ReturnsLongestQualifyingLength(
        string text, int minimumRepeats, int expected)
    {
        var actual = LongestSubstringWithAtLeastKRepeatingCharactersSolution
            .LongestByBruteForce(text, minimumRepeats);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestByDivideAndConquer_Cases_ReturnsLongestQualifyingLength(
        string text, int minimumRepeats, int expected)
    {
        var actual = LongestSubstringWithAtLeastKRepeatingCharactersSolution
            .LongestByDivideAndConquer(text, minimumRepeats);
        Assert.Equal(expected, actual);
    }
}
