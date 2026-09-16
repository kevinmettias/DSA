using DSAExperimentation.LeetCode.PacificAtlanticWaterFlow;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PacificAtlanticWaterFlow;

// Harness only. Both strategies are PacificAtlanticWaterFlowSolution's - this file
// just pins them to LeetCode's published examples, one theory per strategy so a
// failure names the strategy that broke.
public sealed partial class PacificAtlanticWaterFlowTests
{
    public static TheoryData<int[][], (int Row, int Col)[]> Examples =>
        new()
        {
            {
                [
                    [1, 2, 2, 3, 5],
                    [3, 2, 3, 4, 4],
                    [2, 4, 5, 3, 1],
                    [6, 7, 1, 4, 5],
                    [5, 1, 1, 2, 4],
                ],
                [(0, 4), (1, 3), (1, 4), (2, 2), (3, 0), (3, 1), (4, 0)]
            },
            { [[5]], [(0, 0)] },
            { [[3, 3], [3, 3]], [(0, 0), (0, 1), (1, 0), (1, 1)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCellsByPerCellDfs_LeetCodeExamples_ReturnsCellsThatReachBothOceans(
        int[][] heights, (int Row, int Col)[] expected) =>
        Assert.Equal(
            expected.OrderBy(p => p).ToArray(),
            PacificAtlanticWaterFlowSolution.FindCellsByPerCellDfs(heights).OrderBy(p => p).ToArray());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCellsByMultiSourceFloodFill_LeetCodeExamples_ReturnsCellsThatReachBothOceans(
        int[][] heights, (int Row, int Col)[] expected) =>
        Assert.Equal(
            expected.OrderBy(p => p).ToArray(),
            PacificAtlanticWaterFlowSolution.FindCellsByMultiSourceFloodFill(heights).OrderBy(p => p).ToArray());
}
