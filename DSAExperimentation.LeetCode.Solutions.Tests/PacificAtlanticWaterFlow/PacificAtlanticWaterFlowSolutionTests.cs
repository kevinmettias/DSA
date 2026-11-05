using DSAExperimentation.LeetCode.Solutions.PacificAtlanticWaterFlow;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PacificAtlanticWaterFlow;

// Harness only. Both strategies are PacificAtlanticWaterFlowSolution's - this file
// pins them to a single cell and a flat grid, where every cell reaches both oceans,
// one theory per strategy so a failure names the strategy that broke. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class PacificAtlanticWaterFlowSolutionTests
{
    public static TheoryData<int[][], (int Row, int Col)[]> Cases =>
        new()
        {
            { [[5]], [(0, 0)] },
            { [[3, 3], [3, 3]], [(0, 0), (0, 1), (1, 0), (1, 1)] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindCellsByPerCellDfs_Cases_ReturnsCellsThatReachBothOceans(
        int[][] heights, (int Row, int Col)[] expected) =>
        Assert.Equal(
            expected.OrderBy(p => p).ToArray(),
            PacificAtlanticWaterFlowSolution.FindCellsByPerCellDfs(heights).OrderBy(p => p).ToArray());

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindCellsByMultiSourceFloodFill_Cases_ReturnsCellsThatReachBothOceans(
        int[][] heights, (int Row, int Col)[] expected) =>
        Assert.Equal(
            expected.OrderBy(p => p).ToArray(),
            PacificAtlanticWaterFlowSolution.FindCellsByMultiSourceFloodFill(heights).OrderBy(p => p).ToArray());
}
