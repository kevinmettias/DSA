using DSAExperimentation.LeetCode.ExtraCharactersInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExtraCharactersInAString;

// Harness only: both strategies live in ExtraCharactersInAStringSolution and are pinned
// to LeetCode's published examples, plus the degenerate cases the two arms have to agree
// on - a string the dictionary cannot touch at all, an exact single-word cover, a
// dictionary word longer than the string (which the trie arm has to reject by running
// out of string rather than out of prefix), and the greedy trap where taking the longest
// word first is worse than taking the shorter one.
public sealed class ExtraCharactersInAStringTests
{
    public static TheoryData<string, string[], int> Examples =>
        new()
        {
            { "leetscode", ["leet", "code", "leetcode"], 1 },
            { "sayhelloworld", ["hello", "world"], 3 },
            { "abc", ["x", "y"], 3 },
            { "abc", ["abc"], 0 },
            { "ab", ["abc"], 2 },
            { "abcdef", ["abcde", "abc", "def"], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinExtraCharsByHashSetFullScan_LeetCodeExamples_ReturnsFewestLeftoverCharacters(
        string s, string[] dictionary, int expected)
    {
        var actual = ExtraCharactersInAStringSolution.MinExtraCharsByHashSetFullScan(s, dictionary);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinExtraCharsByTriePrunedScan_LeetCodeExamples_ReturnsFewestLeftoverCharacters(
        string s, string[] dictionary, int expected)
    {
        var actual = ExtraCharactersInAStringSolution.MinExtraCharsByTriePrunedScan(s, dictionary);
        Assert.Equal(expected, actual);
    }
}
