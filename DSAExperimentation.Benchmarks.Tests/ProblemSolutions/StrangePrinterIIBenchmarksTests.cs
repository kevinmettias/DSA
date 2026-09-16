using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StrangePrinterIIBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same question - whether LC 1591's color dependency graph is acyclic - one by rescanning every
// remaining color per step, one by Kahn's algorithm proper, so a harness whose arms disagree is
// timing two different problems. Setup's graph is a guaranteed-acyclic chain, so the same color
// count must rebuild the same workload; neither arm mutates the prepared graph (each builds its
// own in-degree bookkeeping), so one harness instance is safe to call twice in either order.
public sealed partial class StrangePrinterIIBenchmarksTests
{
    private const int SmallestColorCount = 50;

    // The benchmark's Setup caps every edge to point from a lower color id to a higher one, so
    // the prepared graph cannot contain a cycle and both arms must report it printable.
    private const bool ExpectedPrintable = true;

    [Fact]
    public void Setup_SameColorCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IsPrintableByKahnsTopologicalSort()),
            AnswerText.Of(BuildHarness().IsPrintableByKahnsTopologicalSort()));

    [Fact]
    public void IsPrintableByNaiveRescan_AgreesWithIsPrintableByKahnsTopologicalSort()
    {
        var harness = BuildHarness();
        var naiveRescan = harness.IsPrintableByNaiveRescan();

        Assert.Equal(naiveRescan, harness.IsPrintableByKahnsTopologicalSort());
        Assert.Equal(ExpectedPrintable, naiveRescan);
    }

    [Fact]
    public void IsPrintableByKahnsTopologicalSort_AgreesWithIsPrintableByNaiveRescan()
    {
        var harness = BuildHarness();
        var kahnsSort = harness.IsPrintableByKahnsTopologicalSort();

        Assert.Equal(kahnsSort, harness.IsPrintableByNaiveRescan());
        Assert.Equal(ExpectedPrintable, kahnsSort);
    }

    private static StrangePrinterIIBenchmarks BuildHarness()
    {
        var harness = new StrangePrinterIIBenchmarks { ColorCount = SmallestColorCount };
        harness.Setup();

        return harness;
    }
}
