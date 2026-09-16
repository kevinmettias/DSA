using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumMovesToSpreadStonesOverGridBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumMovesToSpreadStonesOverGridSolution's, the same methods
// MinimumMovesToSpreadStonesOverGridTests proves correct, and both return the fewest moves that
// leave exactly one stone per cell. Both arms are permutation searches over the same excess
// stones, so arms that disagree are timing two different problems.
//
// Setup's workload is fixed by [Params] rather than seeded - PileCount selects one of two
// hard-coded 3x3 grids - so the same parameters must rebuild the same grid.
public sealed partial class MinimumMovesToSpreadStonesOverGridBenchmarksTests
{
    // The smallest declared [Params] value: it concentrates all six spare stones on one cell,
    // which is the distribution the backtracking arm has to search hardest.
    private const int SmallestPileCount = 1;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BruteForcePermutation(),
            BuildHarness().BruteForcePermutation());

    [Fact]
    public void BruteForcePermutation_AgreesWithBacktrackPermutation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackPermutation(), harness.BruteForcePermutation());
    }

    [Fact]
    public void BacktrackPermutation_AgreesWithBruteForcePermutation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePermutation(), harness.BacktrackPermutation());
    }

    private static MinimumMovesToSpreadStonesOverGridBenchmarks BuildHarness()
    {
        var harness = new MinimumMovesToSpreadStonesOverGridBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
