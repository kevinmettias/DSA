using DSAExperimentation.LeetCode.Solutions.SmallestStringWithSwaps;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestStringWithSwaps;

// Harness only. Both component-discovery strategies are
// SmallestStringWithSwapsSolution's - this file just pins them to the degenerate
// no-pair case, where every index is its own component and the string comes back
// untouched, and to pairs that only join an index to itself, which leave it untouched
// too. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SmallestStringWithSwapsSolutionTests
{
    public static TheoryData<string, int[][], string> Cases =>
        new()
        {
            { "dcab", [], "dcab" },
            { "zyx", [[0, 0], [1, 1]], "zyx" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringByAdjacencyListBfs_Cases_SortsEachComponentIndependently(
        string source, int[][] pairs, string expected)
    {
        var actual = SmallestStringWithSwapsSolution.SmallestStringByAdjacencyListBfs(source, pairs);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringByDisjointSet_Cases_SortsEachComponentIndependently(
        string source, int[][] pairs, string expected)
    {
        var actual = SmallestStringWithSwapsSolution.SmallestStringByDisjointSet(source, pairs);

        Assert.Equal(expected, actual);
    }
}
