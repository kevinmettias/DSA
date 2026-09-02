using DSAExperimentation.LeetCode.ValidAnagram;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidAnagram;

// Harness only. Both strategies are ValidAnagramSolution's - this file just pins
// them to LeetCode's published examples plus the differing-length case.
public sealed class ValidAnagramTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "anagram", "nagaram", true },
            { "rat", "car", false },
            { "aa", "a", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsAnagramByBruteForce_LeetCodeExamples_ReturnsWhetherTIsAnAnagramOfS(
        string s, string t, bool expected) =>
        Assert.Equal(expected, ValidAnagramSolution.IsAnagramByBruteForce(s, t));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsAnagramByHashMapFrequencyCount_LeetCodeExamples_ReturnsWhetherTIsAnAnagramOfS(
        string s, string t, bool expected) =>
        Assert.Equal(expected, ValidAnagramSolution.IsAnagramByHashMapFrequencyCount(s, t));
}
