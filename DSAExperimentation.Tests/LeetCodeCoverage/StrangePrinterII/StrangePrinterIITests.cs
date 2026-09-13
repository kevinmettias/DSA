using DSAExperimentation.LeetCode.StrangePrinterII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StrangePrinterII;

// Harness only. The color graph is StrangePrinterIISolution's own
// ColorNode/ColorTopology and both cycle-detection strategies are its methods;
// this file just pins them to LeetCode's published examples plus the single-color
// and mutually-overlapping degenerate grids. The naive-rescan arm used to live
// inlined in StrangePrinterIIBenchmarks and was asserted by nothing - it is under
// test here for the first time.
public sealed class StrangePrinterIITests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            { [[1, 1, 1, 1], [1, 2, 2, 1], [1, 2, 2, 1], [1, 1, 1, 1]], true },
            { [[1, 1, 1, 1], [1, 1, 3, 3], [1, 1, 3, 4], [5, 5, 1, 4]], true },
            { [[1, 2, 1], [2, 1, 2], [1, 2, 1]], false },
            { [[1, 2, 1], [2, 1, 2]], false },
            { [[1]], true },
            { [[1, 2], [1, 2]], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPrintableByKahnsTopologicalSort_LeetCodeExamples_ReturnsWhetherColorGraphIsAcyclic(
        int[][] targetGrid, bool expected) =>
        Assert.Equal(expected, StrangePrinterIISolution.IsPrintableByKahnsTopologicalSort(targetGrid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPrintableByNaiveRescan_LeetCodeExamples_ReturnsWhetherColorGraphIsAcyclic(
        int[][] targetGrid, bool expected) =>
        Assert.Equal(expected, StrangePrinterIISolution.IsPrintableByNaiveRescan(targetGrid));
}
