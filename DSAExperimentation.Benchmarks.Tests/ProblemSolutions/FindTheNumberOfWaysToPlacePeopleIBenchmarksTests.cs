using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNumberOfWaysToPlacePeopleIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - checking every triple against the maxY sweep
// over the same points sorted by the rule the sweep's precondition names - so a harness whose arms
// disagree is timing two different problems. The arms read two different views of one point set
// (raw and pre-sorted), so agreement is what witnesses that the hoisted sort is the order the
// sweep is defined over. Setup draws the points from one fixed seed, so the same Length must
// rebuild both views.
public sealed partial class FindTheNumberOfWaysToPlacePeopleIBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithSortedSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedSweep(), harness.BruteForce());
    }

    [Fact]
    public void SortedSweep_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SortedSweep());
    }

    private static FindTheNumberOfWaysToPlacePeopleIBenchmarks BuildHarness()
    {
        var harness = new FindTheNumberOfWaysToPlacePeopleIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
