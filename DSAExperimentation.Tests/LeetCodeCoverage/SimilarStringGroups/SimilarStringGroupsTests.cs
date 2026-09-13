using DSAExperimentation.LeetCode.SimilarStringGroups;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SimilarStringGroups;

// Harness only: both grouping strategies live in SimilarStringGroupsSolution and are
// pinned to the same examples here, including the transitive chain (tars~rats~arts)
// that neither direct comparison would join on its own, equal-but-not-swapped
// duplicates, and a set with no similar pair at all.
public sealed class SimilarStringGroupsTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["tars", "rats", "arts", "star"], 2 },
            { ["omv", "ovm"], 1 },
            { ["abc", "abc", "xyz"], 2 },
            { ["abc", "bca", "cab"], 3 },
            { ["kccomwcgcs"], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGroupsByGroupListScan_LeetCodeExamples_ReturnsSimilarityGroupCount(
        string[] strs, int expected) =>
        Assert.Equal(expected, SimilarStringGroupsSolution.CountGroupsByGroupListScan(strs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGroupsByDisjointSet_LeetCodeExamples_ReturnsSimilarityGroupCount(
        string[] strs, int expected) =>
        Assert.Equal(expected, SimilarStringGroupsSolution.CountGroupsByDisjointSet(strs));
}
