using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSequencesToKBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a fresh harness, so this asserts what it cannot: a lower bound on
// the answer that comes from the workload's shape rather than from either arm, and that a harness
// which has already answered once answers as a rebuild does, since BenchmarkDotNet calls an arm many
// times on one harness while the generic check builds a fresh harness for every call. Setup seeds
// nums, so the same Length must rebuild the same array.
public sealed partial class CountSequencesToKBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: the target is fixed at 1, which leaving every element unchanged
        // always reaches, so both arms do real search work instead of short-circuiting on an
        // unreachable target.
        Assert.InRange(first.BruteForceSearch(SmallestLength), 1L, long.MaxValue);
        Assert.Equal(first.BruteForceSearch(SmallestLength), second.BruteForceSearch(SmallestLength));
    }

    private static CountSequencesToKBenchmarks BuildHarness()
    {
        var harness = new CountSequencesToKBenchmarks();
        harness.Setup();

        return harness;
    }
}
