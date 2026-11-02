using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTeamsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing, and to answering the same on a rebuilt harness and when called again
// on one, so this asserts what it cannot: a range for the answer that comes from the workload's
// shape rather than from either arm.
public sealed partial class CountNumberOfTeamsBenchmarksTests
{
    private const int SmallestLength = 80;

    // Every team is one index triple, so no answer can exceed the number of triples the ratings hold.
    private const int MostIndexTriples = SmallestLength * (SmallestLength - 1) * (SmallestLength - 2) / 6;

    // The documented shape: ratings are random draws from a 100_000-wide range, so the array
    // holds many strictly monotone triples for both arms to find, and never more triples than
    // the index triples it contains.
    [Fact]
    public void TripleLoopScan_EightyRandomRatings_CountsWithinTheIndexTriples() =>
        Assert.InRange(BuildHarness().TripleLoopScan(), 1, MostIndexTriples);

    private static CountNumberOfTeamsBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTeamsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
