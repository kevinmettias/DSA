using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KokoEatingBananasBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the slowest eating speed that clears every pile inside
// the hour budget - so a harness whose arms disagree is timing two different problems. The piles
// and the budget are both fixed in [GlobalSetup], so the same Length rebuilds the same question,
// and both arms bisect the same monotone predicate, which is what their answers are compared on.
public sealed partial class KokoEatingBananasBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePilesAndHourBudget() =>
        Assert.Equal(
            BuildHarness().ManualBinarySearch(),
            BuildHarness().ManualBinarySearch());

    [Fact]
    public void ManualBinarySearch_SeededPiles_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.ManualBinarySearch());
    }

    [Fact]
    public void SequenceLowerBound_SeededPiles_AgreesWithManualBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualBinarySearch(), harness.SequenceLowerBound());
    }

    private static KokoEatingBananasBenchmarks BuildHarness()
    {
        var harness = new KokoEatingBananasBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
