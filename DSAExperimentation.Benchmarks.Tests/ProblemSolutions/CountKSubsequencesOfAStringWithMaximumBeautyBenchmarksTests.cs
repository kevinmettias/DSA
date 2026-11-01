using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a lower bound on the answer that comes from the workload's shape rather than from either
// arm, and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds the text, so the same Length must rebuild it.
public sealed partial class CountKSubsequencesOfAStringWithMaximumBeautyBenchmarksTests
{
    private const int SmallestLength = 5_000;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: the text is drawn from a five-letter pool and Setup asks for a
        // subsequence of four distinct characters, so the pool is always large enough to supply
        // one and the count can never be the degenerate 0 an undersized alphabet would give.
        Assert.InRange(first.BruteForceCombinations(), 1L, long.MaxValue);
        Assert.Equal(first.BruteForceCombinations(), second.BruteForceCombinations());
    }

    private static CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks BuildHarness()
    {
        var harness = new CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
