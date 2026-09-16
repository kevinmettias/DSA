using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarraysDistinctElementSumOfSquaresIBenchmarks (ARCHITECTURE 17.9): both
// arms answer the same question - LC 2913's sum of squared distinct counts - one by re-deriving
// each subarray's distinct count with a linear scan, one by growing one set per start index, so
// a harness whose arms disagree is timing two different problems. Setup's values are seeded, so
// the same length must rebuild the same workload.
public sealed partial class SubarraysDistinctElementSumOfSquaresIBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithGrowingSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.GrowingSet());
    }

    [Fact]
    public void GrowingSet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GrowingSet(), harness.BruteForce());
    }

    private static SubarraysDistinctElementSumOfSquaresIBenchmarks BuildHarness()
    {
        var harness = new SubarraysDistinctElementSumOfSquaresIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
