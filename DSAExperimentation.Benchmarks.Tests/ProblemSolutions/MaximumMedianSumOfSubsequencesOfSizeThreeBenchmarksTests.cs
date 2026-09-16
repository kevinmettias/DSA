using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarks (ARCHITECTURE 17.9):
// both arms are MaximumMedianSumOfSubsequencesOfSizeThreeSolution's competing strategies for one
// question - the exhaustive partition search against the sorted greedy - so a harness whose arms
// disagree is timing two different problems. Both answer with a single sum, compared directly.
public sealed partial class MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarksTests
{
    private const int SmallestElementCount = 6;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameValueArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The value array is private, so the rebuild is pinned through the sum it produces: the
        // same ElementCount must draw the same seeded values and score them identically.
        Assert.Equal(first.BruteForce(), second.BruteForce());
        Assert.Equal(first.SortedGreedy(), second.SortedGreedy());
    }

    [Fact]
    public void BruteForce_SeededValueArray_AgreesWithSortedGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedGreedy(), harness.BruteForce());
    }

    [Fact]
    public void SortedGreedy_SeededValueArray_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SortedGreedy());
    }

    private static MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarks BuildHarness()
    {
        var harness = new MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
