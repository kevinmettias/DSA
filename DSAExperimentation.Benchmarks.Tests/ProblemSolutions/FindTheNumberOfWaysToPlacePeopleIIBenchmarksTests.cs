using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNumberOfWaysToPlacePeopleIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - the O(n^3) triple check against the maxY sweep
// over the same points sorted by the rule the sweep's precondition names - so a harness whose arms
// disagree is timing two different problems. The arms read two different views of one point set
// (raw and pre-sorted), so agreement is what witnesses that the hoisted sort is the order the
// sweep is defined over. The sweep only reads the sequence, so one harness is safe to call twice
// in either order.
public sealed partial class FindTheNumberOfWaysToPlacePeopleIIBenchmarksTests
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

    private static FindTheNumberOfWaysToPlacePeopleIIBenchmarks BuildHarness()
    {
        var harness = new FindTheNumberOfWaysToPlacePeopleIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
