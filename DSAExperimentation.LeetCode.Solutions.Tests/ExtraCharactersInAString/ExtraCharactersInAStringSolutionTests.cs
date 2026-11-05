using DSAExperimentation.LeetCode.Solutions.ExtraCharactersInAString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ExtraCharactersInAString;

// Harness only: both strategies live in ExtraCharactersInAStringSolution and are pinned
// to the degenerate cases the two arms have to agree
// on - a string the dictionary cannot touch at all, an exact single-word cover, a
// dictionary word longer than the string (which the trie arm has to reject by running
// out of string rather than out of prefix), and the greedy trap where taking the longest
// word first is worse than taking the shorter one.
public sealed partial class ExtraCharactersInAStringSolutionTests
{
    public static TheoryData<string, string[], int> Cases =>
        new()
        {
            { "abc", ["x", "y"], 3 },
            { "abc", ["abc"], 0 },
            { "ab", ["abc"], 2 },
            { "abcdef", ["abcde", "abc", "def"], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinExtraCharsByHashSetFullScan_Cases_ReturnsFewestLeftoverCharacters(
        string text, string[] dictionary, int expected)
    {
        var actual =
            ExtraCharactersInAStringSolution.MinExtraCharsByHashSetFullScan(text, dictionary);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinExtraCharsByTriePrunedScan_Cases_ReturnsFewestLeftoverCharacters(
        string text, string[] dictionary, int expected)
    {
        var actual =
            ExtraCharactersInAStringSolution.MinExtraCharsByTriePrunedScan(text, dictionary);
        Assert.Equal(expected, actual);
    }
}
