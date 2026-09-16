using DSAExperimentation.LeetCode.SmallestStringWithSwaps;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestStringWithSwaps;

// Harness only. Both component-discovery strategies are
// SmallestStringWithSwapsSolution's - this file just pins them to LeetCode's
// published examples plus the degenerate no-pair case, where every index is its own
// component and the string comes back untouched.
public sealed class SmallestStringWithSwapsTests
{
    public static TheoryData<string, int[][], string> Examples =>
        new()
        {
            { "dcab", [[0, 3], [1, 2]], "bacd" },
            { "dcab", [[0, 3], [1, 2], [0, 2]], "abcd" },
            { "cba", [[0, 1], [1, 2]], "abc" },
            { "dcab", [], "dcab" },
            { "zyx", [[0, 0], [1, 1]], "zyx" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByAdjacencyListBfs_LeetCodeExamples_SortsEachComponentIndependently(
        string source, int[][] pairs, string expected)
    {
        var actual = SmallestStringWithSwapsSolution.SmallestStringByAdjacencyListBfs(source, pairs);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByDisjointSet_LeetCodeExamples_SortsEachComponentIndependently(
        string source, int[][] pairs, string expected)
    {
        var actual = SmallestStringWithSwapsSolution.SmallestStringByDisjointSet(source, pairs);

        Assert.Equal(expected, actual);
    }
}
