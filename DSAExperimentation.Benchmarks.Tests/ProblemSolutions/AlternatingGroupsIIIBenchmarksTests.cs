using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AlternatingGroupsIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// AlternatingGroupsIIISolution's competing strategies for the same question - the O(n) circle scan per size query
// against AlternatingRunLedger's O(log n) prefix lookups - so a harness whose arms disagree is answering a
// different query script. Both arms answer the size queries in the order Setup built them, so the outer order is
// the queries' own and the harness compares positionally. Setup draws the circle and then the intermixed repaint
// and size queries under one seed, so the same Length must rebuild the same script.
public sealed partial class AlternatingGroupsIIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 5_000)] circle lengths.
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_ThousandTileCircle_AgreesWithRunLengthFenwick()
    {
        var harness = BuildHarness();

        Assert.NotEmpty(harness.BruteForce());
        Assert.Equal(AnswerText.Of(harness.RunLengthFenwick()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void RunLengthFenwick_ThousandTileCircle_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.NotEmpty(harness.RunLengthFenwick());
        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.RunLengthFenwick()));
    }

    private static AlternatingGroupsIIIBenchmarks BuildHarness()
    {
        var harness = new AlternatingGroupsIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
