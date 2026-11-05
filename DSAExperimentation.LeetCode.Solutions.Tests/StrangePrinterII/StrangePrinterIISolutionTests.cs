using DSAExperimentation.LeetCode.Solutions.StrangePrinterII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StrangePrinterII;

// Harness only. The color graph is StrangePrinterIISolution's own
// ColorNode/ColorTopology and both cycle-detection strategies are its methods;
// this file pins them to the single-color and mutually-overlapping degenerate
// grids and to two colors side by side that never overlap. The naive-rescan arm
// used to live inlined in StrangePrinterIIBenchmarks and was asserted by nothing -
// it is under test here for the first time. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class StrangePrinterIISolutionTests
{
    public static TheoryData<ColorGridCase> Cases =>
        new()
        {
            new ColorGridCase([[1, 2, 1], [2, 1, 2]], IsPrintable: false),
            new ColorGridCase([[1]], IsPrintable: true),
            new ColorGridCase([[1, 2], [1, 2]], IsPrintable: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsPrintableByKahnsTopologicalSort_Cases_ReturnsWhetherColorGraphIsAcyclic(
        ColorGridCase testCase) =>
        Assert.Equal(testCase.IsPrintable, StrangePrinterIISolution.IsPrintableByKahnsTopologicalSort(testCase.TargetGrid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsPrintableByNaiveRescan_Cases_ReturnsWhetherColorGraphIsAcyclic(
        ColorGridCase testCase) =>
        Assert.Equal(testCase.IsPrintable, StrangePrinterIISolution.IsPrintableByNaiveRescan(testCase.TargetGrid));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct ColorGridCase(int[][] TargetGrid, bool IsPrintable);
}
