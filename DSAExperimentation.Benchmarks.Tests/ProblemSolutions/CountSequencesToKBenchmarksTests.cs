using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSequencesToKBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing, and to answering the same on a rebuilt harness and when called again
// on one, so this asserts what it cannot: a lower bound on the answer that comes from the
// workload's shape rather than from either arm.
public sealed partial class CountSequencesToKBenchmarksTests
{
    private const int SmallestLength = 8;

    // The documented shape: the target is fixed at 1, which leaving every element unchanged
    // always reaches, so both arms do real search work instead of short-circuiting on an
    // unreachable target.
    [Fact]
    public void BruteForceSearch_TargetOne_CountsAtLeastOneSequence() =>
        Assert.InRange(BuildHarness().BruteForceSearch(SmallestLength), 1L, long.MaxValue);

    private static CountSequencesToKBenchmarks BuildHarness()
    {
        var harness = new CountSequencesToKBenchmarks();
        harness.Setup();

        return harness;
    }
}
