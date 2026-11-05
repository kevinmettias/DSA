using DSAExperimentation.LeetCode.Solutions.LongestRepeatingCharacterReplacement;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestRepeatingCharacterReplacement;

// LeetCode 424. Longest Repeating Character Replacement: harness only. All three
// strategies are LongestRepeatingCharacterReplacementSolution's - this file just
// pins them to the empty string, whose longest run is 0 whatever the replacement
// budget. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LongestRepeatingCharacterReplacementSolutionTests
{
    public static TheoryData<string, int, int> Cases =>
        new()
        {
            { "", 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestRunByBruteForce_Cases_ReturnsLongestAchievableRun(
        string text, int maxReplacements, int expected)
    {
        var actual = LongestRepeatingCharacterReplacementSolution.LongestRunByBruteForce(text, maxReplacements);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestRunBySlidingWindowHashMap_Cases_ReturnsLongestAchievableRun(
        string text, int maxReplacements, int expected)
    {
        var actual = LongestRepeatingCharacterReplacementSolution.LongestRunBySlidingWindowHashMap(text, maxReplacements);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestRunBySlidingWindowLetterCounts_Cases_ReturnsLongestAchievableRun(
        string text, int maxReplacements, int expected)
    {
        var actual = LongestRepeatingCharacterReplacementSolution.LongestRunBySlidingWindowLetterCounts(text, maxReplacements);

        Assert.Equal(expected, actual);
    }
}
