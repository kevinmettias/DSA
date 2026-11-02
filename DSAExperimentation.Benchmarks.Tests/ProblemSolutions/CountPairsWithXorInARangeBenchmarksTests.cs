using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPairsWithXorInARangeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing, and to answering the same on a rebuilt harness and when
// called again on one, so this asserts what it cannot: a range for the answer that comes from the
// workload's shape rather than from either arm.
public sealed partial class CountPairsWithXorInARangeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted pair is one index pair, so the answer is bounded by how many the array holds.
    private const int MostIndexPairs = SmallestLength * (SmallestLength - 1) / 2;

    // The documented shape: values are spread over [1, 20_000] while the queried xor range is
    // [100, 5_000], so real pairs land inside it - and none of them can be counted twice.
    [Fact]
    public void PairwiseScan_XorRangeInsideTheValueSpread_CountsWithinTheIndexPairs() =>
        Assert.InRange(BuildHarness().PairwiseScan(), 1, MostIndexPairs);

    private static CountPairsWithXorInARangeBenchmarks BuildHarness()
    {
        var harness = new CountPairsWithXorInARangeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
