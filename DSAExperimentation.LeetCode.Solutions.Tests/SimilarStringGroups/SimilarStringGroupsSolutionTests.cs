using DSAExperimentation.LeetCode.Solutions.SimilarStringGroups;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SimilarStringGroups;

// Harness only: both grouping strategies live in SimilarStringGroupsSolution and are
// pinned to the same cases here, including the transitive chain (tars~rats~arts)
// that neither direct comparison would join on its own, equal-but-not-swapped
// duplicates, and a set with no similar pair at all.
public sealed partial class SimilarStringGroupsSolutionTests
{
    public static TheoryData<string[], int> Cases =>
        new()
        {
            { ["abc", "abc", "xyz"], 2 },
            { ["abc", "bca", "cab"], 3 },
            { ["kccomwcgcs"], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGroupsByGroupListScan_Cases_ReturnsSimilarityGroupCount(
        string[] strs, int expected) =>
        Assert.Equal(expected, SimilarStringGroupsSolution.CountGroupsByGroupListScan(strs));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGroupsByDisjointSet_Cases_ReturnsSimilarityGroupCount(
        string[] strs, int expected) =>
        Assert.Equal(expected, SimilarStringGroupsSolution.CountGroupsByDisjointSet(strs));
}
