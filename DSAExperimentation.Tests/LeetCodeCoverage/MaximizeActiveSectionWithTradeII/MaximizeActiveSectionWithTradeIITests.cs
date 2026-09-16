using DSAExperimentation.LeetCode.MaximizeActiveSectionWithTradeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeActiveSectionWithTradeII;

// Harness only. Both strategies are MaximizeActiveSectionWithTradeIISolution's
// - this file just pins them to LeetCode's published examples.
public sealed partial class MaximizeActiveSectionWithTradeIITests
{
    public static TheoryData<string, int[][], int[]> Examples =>
        new()
        {
            { "01", [[0, 1]], [1] },
            { "0100", [[0, 3], [0, 2], [1, 3], [2, 3]], [4, 3, 1, 1] },
            { "1000100", [[1, 5], [0, 6], [0, 4]], [6, 7, 2] },
            { "01010", [[0, 3], [1, 4], [1, 3]], [4, 4, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxActiveAfterTradeByRunScan_LeetCodeExamples_ReturnsBestTradeCountPerQuery(
        string text, int[][] queries, int[] expected)
    {
        var actual = MaximizeActiveSectionWithTradeIISolution.MaxActiveAfterTradeByRunScan(text, queries);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxActiveAfterTradeByRangeMaxIndex_LeetCodeExamples_ReturnsBestTradeCountPerQuery(
        string text, int[][] queries, int[] expected)
    {
        var actual = MaximizeActiveSectionWithTradeIISolution.MaxActiveAfterTradeByRangeMaxIndex(text, queries);
        Assert.Equal(expected, actual);
    }
}
