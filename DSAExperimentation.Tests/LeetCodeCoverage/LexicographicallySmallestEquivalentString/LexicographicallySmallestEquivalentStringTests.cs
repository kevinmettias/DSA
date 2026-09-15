using DSAExperimentation.LeetCode.LexicographicallySmallestEquivalentString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestEquivalentString;

// Harness only. Both strategies are LexicographicallySmallestEquivalentStringSolution's -
// the adjacency-list BFS baseline and this repo's DisjointSet composition - pinned here
// to LeetCode's published examples plus a self-equivalent pair and a baseStr whose
// letters were never mentioned by s1/s2 at all.
public sealed class LexicographicallySmallestEquivalentStringTests
{
    public static TheoryData<string, string, string, string> Examples =>
        new()
        {
            { "parker", "morris", "parser", "makkek" },
            { "hello", "world", "hold", "hdld" },
            { "leetcode", "programs", "sourcecode", "aauaaaaada" },
            { "ab", "ba", "ab", "aa" },
            { "a", "b", "cz", "cz" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestEquivalentStringByAdjacencyListBfs_LeetCodeExamples_ReturnsRemappedString(
        string s1, string s2, string baseStr, string expected) =>
        Assert.Equal(
            expected,
            LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByAdjacencyListBfs(
                s1,
                s2,
                new LexicographicallySmallestEquivalentStringSolution.BaseText(baseStr)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestEquivalentStringByDisjointSet_LeetCodeExamples_ReturnsRemappedString(
        string s1, string s2, string baseStr, string expected) =>
        Assert.Equal(
            expected,
            LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByDisjointSet(
                s1,
                s2,
                new LexicographicallySmallestEquivalentStringSolution.BaseText(baseStr)));
}
