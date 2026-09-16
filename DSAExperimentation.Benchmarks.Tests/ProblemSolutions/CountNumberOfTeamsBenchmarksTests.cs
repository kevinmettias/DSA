using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTeamsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the cubic triple loop against the pair of coordinate-compressed
// Fenwick sweeps - so a harness whose arms disagree is timing two different problems. Setup seeds
// the ratings, so the same Length must rebuild the same array.
public sealed partial class CountNumberOfTeamsBenchmarksTests
{
    private const int SmallestLength = 80;

    // Every team is one index triple, so no answer can exceed the number of triples the ratings hold.
    private const int MostIndexTriples = SmallestLength * (SmallestLength - 1) * (SmallestLength - 2) / 6;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: ratings are random draws from a 100_000-wide range, so the array
        // holds many strictly monotone triples for both arms to find, and never more triples than
        // the index triples it contains.
        Assert.InRange(first.TripleLoopScan(), 1, MostIndexTriples);
        Assert.Equal(first.TripleLoopScan(), second.TripleLoopScan());
    }

    [Fact]
    public void TripleLoopScan_WideRatingRange_AgreesWithFenwickTreeSweeps()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweeps(), harness.TripleLoopScan());
    }

    [Fact]
    public void FenwickTreeSweeps_WideRatingRange_AgreesWithTripleLoopScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TripleLoopScan(), harness.FenwickTreeSweeps());
    }

    private static CountNumberOfTeamsBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTeamsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
