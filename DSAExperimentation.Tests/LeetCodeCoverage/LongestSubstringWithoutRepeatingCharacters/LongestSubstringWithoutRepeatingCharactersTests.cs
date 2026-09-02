using DSAExperimentation.LeetCode.LongestSubstringWithoutRepeatingCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringWithoutRepeatingCharacters;

// Harness only. Both strategies are
// LongestSubstringWithoutRepeatingCharactersSolution's - this file pins them to
// LeetCode's published examples.
public sealed class LongestSubstringWithoutRepeatingCharactersTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "abcabcbb", 3 },
            { "bbbbb", 1 },
            { "pwwkew", 3 },
            { "", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthByBruteForce_LeetCodeExamples_ReturnsLongestUniqueRun(string text, int expected) =>
        Assert.Equal(expected, LongestSubstringWithoutRepeatingCharactersSolution.FindLengthByBruteForce(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthBySlidingWindowHashMap_LeetCodeExamples_ReturnsLongestUniqueRun(string text, int expected) =>
        Assert.Equal(
            expected,
            LongestSubstringWithoutRepeatingCharactersSolution.FindLengthBySlidingWindowHashMap(text));
}
