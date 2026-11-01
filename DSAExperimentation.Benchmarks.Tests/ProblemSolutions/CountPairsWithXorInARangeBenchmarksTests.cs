using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPairsWithXorInARangeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a fresh harness, so this asserts what it cannot: a range for
// the answer that comes from the workload's shape rather than from either arm, and that a harness
// which has already answered once answers as a rebuild does, since BenchmarkDotNet calls an arm many
// times on one harness while the generic check builds a fresh harness for every call. Setup seeds
// the values, so the same Length must rebuild the same array.
public sealed partial class CountPairsWithXorInARangeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted pair is one index pair, so the answer is bounded by how many the array holds.
    private const int MostIndexPairs = SmallestLength * (SmallestLength - 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: values are spread over [1, 20_000] while the queried xor range is
        // [100, 5_000], so real pairs land inside it - and none of them can be counted twice.
        Assert.InRange(first.PairwiseScan(), 1, MostIndexPairs);
        Assert.Equal(first.PairwiseScan(), second.PairwiseScan());
    }

    private static CountPairsWithXorInARangeBenchmarks BuildHarness()
    {
        var harness = new CountPairsWithXorInARangeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
