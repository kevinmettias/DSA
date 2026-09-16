using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBuildingWhereAliceAndBobCanMeetBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a per-query brute-force scan against an
// offline sweep that buckets each query at its own upper index - so a harness whose arms disagree is
// answering two different query batches. Both arms return one building index per query, so the
// position in the returned array IS the query and the arrays are compared as ordered sequences.
// Setup draws the heights and the queries from one shared seeded stream, so the same Length must
// rebuild both, with exactly one answer per query drawn.
public sealed partial class FindBuildingWhereAliceAndBobCanMeetBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameHeightsAndQueries()
    {
        Assert.Equal(SmallestLength, BuildHarness().BruteForce().Length);

        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));
    }

    [Fact]
    public void BruteForce_SeededHeightAndQueryBatch_AgreesWithOfflineHeapSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.OfflineHeapSweep()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void OfflineHeapSweep_SeededHeightAndQueryBatch_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.OfflineHeapSweep()));
    }

    private static FindBuildingWhereAliceAndBobCanMeetBenchmarks BuildHarness()
    {
        var harness = new FindBuildingWhereAliceAndBobCanMeetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
