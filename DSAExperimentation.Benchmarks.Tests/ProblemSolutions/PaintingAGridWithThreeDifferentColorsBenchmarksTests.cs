using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PaintingAGridWithThreeDifferentColorsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one count - the full-grid brute force against the column-pattern
// dynamic program - so a harness whose arms disagree is timing two different problems. The whole
// input is the two dimensions and the class has neither a [GlobalSetup] nor any other tuned property,
// so a harness is a bare initializer plus the tuned Columns; there is nothing to set up, and the
// smallest tuned Columns is the one the 3^(rows*columns) brute force can still afford.
public sealed partial class PaintingAGridWithThreeDifferentColorsBenchmarksTests
{
    private const int SmallestColumnCount = 3;

    [Fact]
    public void BruteForceFullGrid_SmallestColumnCount_AgreesWithColumnPatternDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ColumnPatternDynamicProgramming(), harness.BruteForceFullGrid());
    }

    [Fact]
    public void ColumnPatternDynamicProgramming_SmallestColumnCount_AgreesWithBruteForceFullGrid()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceFullGrid(), harness.ColumnPatternDynamicProgramming());
    }

    private static PaintingAGridWithThreeDifferentColorsBenchmarks BuildHarness() =>
        new() { Columns = SmallestColumnCount };
}
