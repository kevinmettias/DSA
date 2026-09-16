using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SelectCellsInGridWithMaximumScoreBenchmarks (ARCHITECTURE 17.9): both arms are
// the same problem's strategies for one grid - the row-by-row recursion and the memoized DP over the
// rows-by-value grouping that Setup prepares alongside it - so a harness whose arms disagree is
// timing two different grids. Setup draws the grid from one fixed seed, so the same GridSize must
// rebuild the same grid and the same grouping; neither arm mutates either one, so one harness
// instance is safe to read twice in either order.
public sealed partial class SelectCellsInGridWithMaximumScoreBenchmarksTests
{
    private const int SmallestGridSize = 4;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_SeededLatticeGrid_AgreesWithBitmaskMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemoization(), harness.BruteForceRecursion());
    }

    [Fact]
    public void BitmaskMemoization_SeededLatticeGrid_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.BitmaskMemoization());
    }

    private static SelectCellsInGridWithMaximumScoreBenchmarks BuildHarness()
    {
        var harness = new SelectCellsInGridWithMaximumScoreBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
