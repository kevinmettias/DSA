using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumIVInputIsABSTBenchmarks (ARCHITECTURE 17.9): the target is deliberately
// unreachable (every node value is non-negative, the target is -1), the same TwoSumBenchmarks
// convention, so both arms answer false on every workload this harness can build - known without
// consulting either arm, so the baseline is pinned to it, where the generic arm check's agreement
// would also hold for two arms that both reported a pair. Both [Benchmark] return types are bool, so
// the pair itself is not observable from here, and tightening that is a harness decision rather than
// this file's.
public sealed partial class TwoSumIVInputIsABSTBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] node counts.
    private const int SmallestNodeCount = 200;

    // Setup's documented outcome: no two non-negative node values sum to the negative target.
    private const bool ExpectedHasPair = false;

    [Fact]
    public void HasTargetPairByNestedPairScan_SmallestNodeCount_FindsNoPair() =>
        Assert.Equal(ExpectedHasPair, BuildHarness().HasTargetPairByNestedPairScan());

    private static TwoSumIVInputIsABSTBenchmarks BuildHarness()
    {
        var harness = new TwoSumIVInputIsABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
