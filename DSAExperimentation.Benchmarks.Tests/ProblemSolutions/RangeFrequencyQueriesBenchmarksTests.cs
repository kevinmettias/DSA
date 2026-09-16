using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeFrequencyQueriesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - how many times a value occurs in each queried range
// - so a harness whose arms disagree is timing two different problems. Both arms sum their
// per-query counts into one running total, so the comparison is a direct scalar one. Setup draws
// the array and the query batch from one fixed seed, so the same Length must rebuild the same pair.
public sealed partial class RangeFrequencyQueriesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRescan(), BuildHarness().BruteForceRescan());

    [Fact]
    public void BruteForceRescan_SeededQueryBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRescan(), harness.HashMapWithBinarySearch());
    }

    [Fact]
    public void HashMapWithBinarySearch_SeededQueryBatch_AgreesWithTheBruteForceArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapWithBinarySearch(), harness.BruteForceRescan());
    }

    private static RangeFrequencyQueriesBenchmarks BuildHarness()
    {
        var harness = new RangeFrequencyQueriesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
