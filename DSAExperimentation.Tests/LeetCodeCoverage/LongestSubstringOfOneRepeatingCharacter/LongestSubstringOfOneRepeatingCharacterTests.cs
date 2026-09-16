using DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringOfOneRepeatingCharacter;

// Harness only. Both strategies are LongestSubstringOfOneRepeatingCharacterSolution's -
// this file just pins them to LeetCode's published examples plus the edge cases the
// RunSegment merge has to get right: a one-character string, an update that splits an
// existing run, and an update that rewrites a character with itself.
public sealed class LongestSubstringOfOneRepeatingCharacterTests
{
    public static TheoryData<RepeatingRunExample> Examples =>
        new()
        {
            { new RepeatingRunExample(S: "babacc", QueryCharacters: "bcb", QueryIndices: [1, 3, 3], Expected: [3, 3, 4]) },
            { new RepeatingRunExample(S: "abyzz", QueryCharacters: "aa", QueryIndices: [2, 1], Expected: [2, 3]) },
            { new RepeatingRunExample(S: "a", QueryCharacters: "b", QueryIndices: [0], Expected: [1]) },

            // "abaa": the surviving "aa" at indices 2-3 beats the leftover "a" at 0.
            { new RepeatingRunExample(S: "aaaa", QueryCharacters: "b", QueryIndices: [1], Expected: [2]) },
            { new RepeatingRunExample(S: "aaa", QueryCharacters: "a", QueryIndices: [1], Expected: [3]) },

            // The final character is already 'e', so the fourth query - not the fifth - is
            // what closes the whole string into one run.
            { new RepeatingRunExample(S: "abcde", QueryCharacters: "eeeee", QueryIndices: [0, 1, 2, 3, 4], Expected: [1, 2, 3, 5, 5]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRepeatingByLinearRescan_LeetCodeExamples_ReturnsLongestRunAfterEachQuery(
        RepeatingRunExample example)
    {
        var actual = LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingByLinearRescan(
            new LongestSubstringOfOneRepeatingCharacterSolution.BaseText(example.S),
            new LongestSubstringOfOneRepeatingCharacterSolution.ReplacementCharacters(example.QueryCharacters),
            example.QueryIndices);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestRepeatingBySegmentTree_LeetCodeExamples_ReturnsLongestRunAfterEachQuery(
        RepeatingRunExample example)
    {
        var actual = LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingBySegmentTree(
            new LongestSubstringOfOneRepeatingCharacterSolution.BaseText(example.S),
            new LongestSubstringOfOneRepeatingCharacterSolution.ReplacementCharacters(example.QueryCharacters),
            example.QueryIndices);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument. The text and the replacement characters are both
    // strings, so a multi-parameter signature let a row be written with those two
    // swapped and still compile; the fields named at each row below say which is which.
    public readonly record struct RepeatingRunExample(
        string S, string QueryCharacters, int[] QueryIndices, int[] Expected);
}
