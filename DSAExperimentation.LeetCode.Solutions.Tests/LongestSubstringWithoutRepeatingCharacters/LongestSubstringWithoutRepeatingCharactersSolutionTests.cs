using DSAExperimentation.LeetCode.Solutions.LongestSubstringWithoutRepeatingCharacters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestSubstringWithoutRepeatingCharacters;

// Harness only. Both strategies are
// LongestSubstringWithoutRepeatingCharactersSolution's - this file pins them to the
// empty string, whose longest run is 0. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class LongestSubstringWithoutRepeatingCharactersSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLengthByBruteForce_Cases_ReturnsLongestUniqueRun(string text, int expected) =>
        Assert.Equal(expected, LongestSubstringWithoutRepeatingCharactersSolution.FindLengthByBruteForce(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLengthBySlidingWindowHashMap_Cases_ReturnsLongestUniqueRun(string text, int expected) =>
        Assert.Equal(
            expected,
            LongestSubstringWithoutRepeatingCharactersSolution.FindLengthBySlidingWindowHashMap(text));
}
