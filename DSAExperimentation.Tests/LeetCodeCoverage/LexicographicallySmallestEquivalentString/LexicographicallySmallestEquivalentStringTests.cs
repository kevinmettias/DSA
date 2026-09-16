using DSAExperimentation.LeetCode.LexicographicallySmallestEquivalentString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestEquivalentString;

// Harness only. Both strategies are LexicographicallySmallestEquivalentStringSolution's -
// the adjacency-list BFS baseline and this repo's DisjointSet composition - pinned here
// to LeetCode's published examples plus a self-equivalent pair and a baseStr whose
// letters were never mentioned by s1/s2 at all.
public sealed class LexicographicallySmallestEquivalentStringTests
{
    public static TheoryData<RemapExample> Examples =>
        new()
        {
            { new RemapExample(S1: "parker", S2: "morris", BaseStr: "parser", Expected: "makkek") },
            { new RemapExample(S1: "hello", S2: "world", BaseStr: "hold", Expected: "hdld") },
            { new RemapExample(S1: "leetcode", S2: "programs", BaseStr: "sourcecode", Expected: "aauaaaaada") },
            { new RemapExample(S1: "ab", S2: "ba", BaseStr: "ab", Expected: "aa") },
            { new RemapExample(S1: "a", S2: "b", BaseStr: "cz", Expected: "cz") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestEquivalentStringByAdjacencyListBfs_LeetCodeExamples_ReturnsRemappedString(
        RemapExample example)
    {
        var baseText = new LexicographicallySmallestEquivalentStringSolution.BaseText(example.BaseStr);
        var actual = LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByAdjacencyListBfs(
            example.S1, example.S2, baseText);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestEquivalentStringByDisjointSet_LeetCodeExamples_ReturnsRemappedString(
        RemapExample example)
    {
        var baseText = new LexicographicallySmallestEquivalentStringSolution.BaseText(example.BaseStr);
        var actual = LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByDisjointSet(
            example.S1, example.S2, baseText);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two equivalence lists, the text they are applied to,
    // and the remapped result. `s1`/`s2` are a symmetric pair - `(s1[i], s2[i])` states
    // one equivalence relation, so swapping them describes the same classes - but
    // `baseStr` is not one of them and `expected` is the answer, so four adjacent
    // `string` positions would let a caller swap either across roles unread.
    public readonly record struct RemapExample(string S1, string S2, string BaseStr, string Expected);
}
