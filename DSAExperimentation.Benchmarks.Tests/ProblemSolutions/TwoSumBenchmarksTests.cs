using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumBenchmarks (ARCHITECTURE 17.9): TwoSumWorkloads plants the one pair
// LC 1 promises at the last two positions, which is what forces both strategies through a worst-case
// scan. It is also why both arms answer true on every workload this harness can build - known
// without consulting either arm, so the baseline is pinned to it, where the generic arm check's
// agreement would also hold for two arms that both missed the pair. With both [Benchmark] return
// types bool and the matched indices discarded into out parameters, the pair itself is not
// observable from this harness; that it is the only one, and the last, is TwoSumWorkloadsTests' to
// show.
public sealed partial class TwoSumBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's documented outcome: the planted pair sums to the target.
    private const bool ExpectedHasPair = true;

    [Fact]
    public void TryFindIndicesByBruteForce_SmallestLength_FindsThePlantedPair() =>
        Assert.Equal(ExpectedHasPair, BuildHarness().TryFindIndicesByBruteForce());

    private static TwoSumBenchmarks BuildHarness()
    {
        var harness = new TwoSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
