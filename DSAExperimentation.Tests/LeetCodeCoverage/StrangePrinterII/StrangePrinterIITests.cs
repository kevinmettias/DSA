using DSAExperimentation.LeetCode.StrangePrinterII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StrangePrinterII;

// Harness only. The color graph is StrangePrinterIISolution's own
// ColorNode/ColorTopology and both cycle-detection strategies are its methods;
// this file just pins them to LeetCode's published examples plus the single-color
// and mutually-overlapping degenerate grids. The naive-rescan arm used to live
// inlined in StrangePrinterIIBenchmarks and was asserted by nothing - it is under
// test here for the first time.
public sealed partial class StrangePrinterIITests
{
    public static TheoryData<ColorGridExample> Examples =>
        new()
        {
            new ColorGridExample([[1, 1, 1, 1], [1, 2, 2, 1], [1, 2, 2, 1], [1, 1, 1, 1]], IsPrintable: true),
            new ColorGridExample([[1, 1, 1, 1], [1, 1, 3, 3], [1, 1, 3, 4], [5, 5, 1, 4]], IsPrintable: true),
            new ColorGridExample([[1, 2, 1], [2, 1, 2], [1, 2, 1]], IsPrintable: false),
            new ColorGridExample([[1, 2, 1], [2, 1, 2]], IsPrintable: false),
            new ColorGridExample([[1]], IsPrintable: true),
            new ColorGridExample([[1, 2], [1, 2]], IsPrintable: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPrintableByKahnsTopologicalSort_LeetCodeExamples_ReturnsWhetherColorGraphIsAcyclic(
        ColorGridExample example) =>
        Assert.Equal(example.IsPrintable, StrangePrinterIISolution.IsPrintableByKahnsTopologicalSort(example.TargetGrid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPrintableByNaiveRescan_LeetCodeExamples_ReturnsWhetherColorGraphIsAcyclic(
        ColorGridExample example) =>
        Assert.Equal(example.IsPrintable, StrangePrinterIISolution.IsPrintableByNaiveRescan(example.TargetGrid));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct ColorGridExample(int[][] TargetGrid, bool IsPrintable);
}
