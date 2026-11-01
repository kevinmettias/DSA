using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StrangePrinterIIBenchmarks (ARCHITECTURE 17.9): Setup's color dependency
// graph is a guaranteed-acyclic chain, so whether LC 1591's grid is printable is known without
// running either arm, and the baseline is asserted against it - the generic arm check only holds
// the Kahn's arm to the baseline's answer, which two arms sharing one wrong verdict would pass.
public sealed partial class StrangePrinterIIBenchmarksTests
{
    private const int SmallestColorCount = 50;

    // The benchmark's Setup caps every edge to point from a lower color id to a higher one, so
    // the prepared graph cannot contain a cycle and both arms must report it printable.
    private const bool ExpectedPrintable = true;

    [Fact]
    public void IsPrintableByNaiveRescan_AcyclicChain_ReportsPrintable() =>
        Assert.Equal(ExpectedPrintable, BuildHarness().IsPrintableByNaiveRescan());

    private static StrangePrinterIIBenchmarks BuildHarness()
    {
        var harness = new StrangePrinterIIBenchmarks { ColorCount = SmallestColorCount };
        harness.Setup();

        return harness;
    }
}
