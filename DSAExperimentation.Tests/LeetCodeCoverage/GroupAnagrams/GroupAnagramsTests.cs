using DSAExperimentation.LeetCode.GroupAnagrams;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GroupAnagrams;

// Harness only. Both strategies are GroupAnagramsSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class GroupAnagramsTests
{
    public static TheoryData<string[], string[][]> Examples =>
        new()
        {
            {
                ["eat", "tea", "tan", "ate", "nat", "bat"],
                [["bat"], ["nat", "tan"], ["ate", "eat", "tea"]]
            },
            { [""], [[""]] },
            { ["a"], [["a"]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupByDictionary_LeetCodeExamples_GroupsWordsBySortedLetters(
        string[] values, string[][] expectedGroups) =>
        AssertGroups(expectedGroups, GroupAnagramsSolution.GroupByDictionary(values));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupByHashMap_LeetCodeExamples_GroupsWordsBySortedLetters(
        string[] values, string[][] expectedGroups) =>
        AssertGroups(expectedGroups, GroupAnagramsSolution.GroupByHashMap(values));

    private static void AssertGroups(string[][] expectedGroups, List<List<string>> actualGroups)
    {
        Assert.Equal(expectedGroups.Length, actualGroups.Count);

        foreach (var expectedGroup in expectedGroups)
        {
            Assert.Contains(
                actualGroups, group => group.Order().SequenceEqual(expectedGroup.Order()));
        }
    }
}
