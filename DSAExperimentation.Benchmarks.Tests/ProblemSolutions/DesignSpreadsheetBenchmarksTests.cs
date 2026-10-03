using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSpreadsheetBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: a
// bound on every value the replay reads, which follows from the script's construction rather than from either arm.
// Setup builds the whole call script from one fixed seed, and each arm returns every value GetValue reported, in
// order.
public sealed partial class DesignSpreadsheetBenchmarksTests
{
    private const int SmallestCellCount = 200;

    // Mirrors the benchmark's own value ceiling: every cell value and every literal is drawn below
    // it, and a formula is at most two such values added together.
    private const int ValueUpperBound = 100_000;

    private const int FormulaTermCount = 2;

    private const int MinimumValue = 0;

    [Fact]
    public void Dictionary_SetAndFormulaScript_ReadsOnlySumsOfTwoDrawnValues() =>
        AssertReadsOnlySumsOfTwoDrawnValues(BuildHarness().Dictionary());

    [Fact]
    public void HashMap_SetAndFormulaScript_ReadsOnlySumsOfTwoDrawnValues() =>
        AssertReadsOnlySumsOfTwoDrawnValues(BuildHarness().HashMap());

    // A cell value and a formula's literal are both drawn from zero up to ValueUpperBound, and every
    // formula is either two cells or one cell and one literal - so each of the CellCount reads reports
    // a non-negative number below twice that ceiling. Nothing else about the script is readable
    // through the public surface.
    private static void AssertReadsOnlySumsOfTwoDrawnValues(int[] values) =>
        Assert.All(values, value => Assert.InRange(value, MinimumValue, (FormulaTermCount * ValueUpperBound) - 1));

    private static DesignSpreadsheetBenchmarks BuildHarness()
    {
        var harness = new DesignSpreadsheetBenchmarks { CellCount = SmallestCellCount };
        harness.Setup();

        return harness;
    }
}
