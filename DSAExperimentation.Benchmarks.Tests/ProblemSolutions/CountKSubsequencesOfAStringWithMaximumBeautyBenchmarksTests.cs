using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a lower bound on the
// answer that comes from the workload's shape rather than from either arm.
public sealed partial class CountKSubsequencesOfAStringWithMaximumBeautyBenchmarksTests
{
    private const int SmallestLength = 5_000;

    // The documented shape: the text is drawn from a five-letter pool and Setup asks for a
    // subsequence of four distinct characters, so the pool is always large enough to supply
    // one and the count can never be the degenerate 0 an undersized alphabet would give.
    [Fact]
    public void BruteForceCombinations_FiveLetterPool_CountsAtLeastOneSubsequence() =>
        Assert.InRange(BuildHarness().BruteForceCombinations(), 1L, long.MaxValue);

    private static CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks BuildHarness()
    {
        var harness = new CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
