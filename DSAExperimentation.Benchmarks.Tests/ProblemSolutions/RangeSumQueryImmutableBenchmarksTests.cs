using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeSumQueryImmutableBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the sum of every queried subarray - so a harness
// whose arms disagree is timing two different problems. Both arms sum their per-query answers into
// one running total, so the comparison is a direct scalar one, and the composed arm's tree is built
// inside the measured call rather than shared, so one harness is safe to call twice. Setup draws
// the array and the query batch from one fixed seed, so the same Length must rebuild the same pair.
public sealed partial class RangeSumQueryImmutableBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRescan(), BuildHarness().BruteForceRescan());

    [Fact]
    public void BruteForceRescan_SeededRangeBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRescan(), harness.FenwickTreeQuery());
    }

    [Fact]
    public void FenwickTreeQuery_SeededRangeBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeQuery(), harness.BruteForceRescan());
    }

    private static RangeSumQueryImmutableBenchmarks BuildHarness()
    {
        var harness = new RangeSumQueryImmutableBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
