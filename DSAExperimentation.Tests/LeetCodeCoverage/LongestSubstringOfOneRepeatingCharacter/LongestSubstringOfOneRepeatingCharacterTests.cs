using DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringOfOneRepeatingCharacter;

// Harness only. Both strategies are LongestSubstringOfOneRepeatingCharacterSolution's -
// this file just pins them to LeetCode's published examples plus the edge cases the
// RunSegment merge has to get right: a one-character string, an update that splits an
// existing run, and an update that rewrites a character with itself.
public sealed class LongestSubstringOfOneRepeatingCharacterTests
{
    public static TheoryData<string, string, int[], int[]> Examples =>
        new()
        {
            { "babacc", "bcb", [1, 3, 3], [3, 3, 4] },
            { "abyzz", "aa", [2, 1], [2, 3] },
            { "a", "b", [0], [1] },
            // "abaa": the surviving "aa" at indices 2-3 beats the leftover "a" at 0.
            { "aaaa", "b", [1], [2] },
            { "aaa", "a", [1], [3] },
            // The final character is already 'e', so the fourth query - not the fifth - is
            // what closes the whole string into one run.
            { "abcde", "eeeee", [0, 1, 2, 3, 4], [1, 2, 3, 5, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRepeatingByLinearRescan_LeetCodeExamples_ReturnsLongestRunAfterEachQuery(
        string s, string queryCharacters, int[] queryIndices, int[] expected) =>
        Assert.Equal(
            expected,
            LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingByLinearRescan(s, queryCharacters, queryIndices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRepeatingBySegmentTree_LeetCodeExamples_ReturnsLongestRunAfterEachQuery(
        string s, string queryCharacters, int[] queryIndices, int[] expected) =>
        Assert.Equal(
            expected,
            LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingBySegmentTree(s, queryCharacters, queryIndices));
}
