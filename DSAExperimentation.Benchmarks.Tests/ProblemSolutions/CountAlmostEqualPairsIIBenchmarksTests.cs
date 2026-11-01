using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAlmostEqualPairsIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only the count the
// workload's documented shape decides: Setup draws its values across LC 3267's whole value range, and
// no two random seven-digit numbers land within the swap budget, so both arms scan every pair
// without ever reaching a match.
public sealed partial class CountAlmostEqualPairsIIBenchmarksTests
{
    private const int SmallestLength = 10;

    // Random seven-digit values are never within two digit swaps of one another.
    private const long ExpectedMatchingPairs = 0L;

    [Fact]
    public void BoundedSwapBacktrack_TenRandomValues_FindsNoMatchingPair() =>
        Assert.Equal(ExpectedMatchingPairs, BuildHarness().BoundedSwapBacktrack());

    private static CountAlmostEqualPairsIIBenchmarks BuildHarness()
    {
        var harness = new CountAlmostEqualPairsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
