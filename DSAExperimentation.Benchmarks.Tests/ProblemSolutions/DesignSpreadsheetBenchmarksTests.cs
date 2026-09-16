using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSpreadsheetBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a BCL dictionary of cells against this repo's own HashMap -
// so a harness whose arms disagree is timing two different problems. Setup builds the whole call
// script from one fixed seed, so the same CellCount must rebuild the same script, and that script
// has to leave every cell and literal it reads inside the range it drew them from.
public sealed partial class DesignSpreadsheetBenchmarksTests
{
    private const int SmallestCellCount = 200;

    // Mirrors the benchmark's own value ceiling: every cell value and every literal is drawn below
    // it, and a formula is at most two such values added together.
    private const int ValueUpperBound = 100_000;

    private const long MinimumValueSum = 0;

    private const long MaximumValueSum = 2L * ValueUpperBound * SmallestCellCount;

    [Fact]
    public void Setup_SameCellCount_RebuildsTheSameCallScript()
    {
        // A cell value and a formula's literal are both drawn from zero up to ValueUpperBound, and
        // every formula is either two cells or one cell and one literal - so each of the
        // CellCount reads reports a non-negative number below twice that ceiling, and the summed
        // report of them all stays inside the band. Nothing else about the script is readable
        // through the public surface.
        Assert.InRange(BuildHarness().Dictionary(), MinimumValueSum, MaximumValueSum);
        Assert.Equal(BuildHarness().Dictionary(), BuildHarness().Dictionary());
    }

    [Fact]
    public void Dictionary_SetAndFormulaScript_AgreesWithHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMap(), harness.Dictionary());
    }

    [Fact]
    public void HashMap_SetAndFormulaScript_AgreesWithDictionary()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Dictionary(), harness.HashMap());
    }

    private static DesignSpreadsheetBenchmarks BuildHarness()
    {
        var harness = new DesignSpreadsheetBenchmarks { CellCount = SmallestCellCount };
        harness.Setup();

        return harness;
    }
}
