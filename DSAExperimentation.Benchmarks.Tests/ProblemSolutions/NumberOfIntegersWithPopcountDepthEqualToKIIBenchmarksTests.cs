using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - mutating a plain copy of nums and
// rescanning each queried range against this repo's own Fenwick-bucket index - so a harness whose arms
// disagree is replaying two different query streams. Setup draws the seeded nums and query stream, so
// the same ElementCount must rebuild the same values and the same queries.
//
// This is one of the stateful classes where the re-preparation has to be checked rather than assumed:
// the composed arm MUTATES the index it is handed, so a second call without an intervening
// [IterationSetup] would replay the queries against an index carried forward from the first run - the
// IterationSetup test below pins exactly that down. The baseline arm neither reads nor writes that
// index, so one harness can still serve both arms once each, in either order.
public sealed partial class NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarksTests
{
    private const int SmallestElementCount = 200;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void IterationSetup_AfterAGradedRun_RePresentsThePristineWorkload()
    {
        var reference = BuildHarness();
        var graded = BuildHarness();
        var expectedScan = AnswerText.Of(reference.BruteForce());
        var expectedBuckets = AnswerText.Of(reference.FenwickBuckets());

        graded.BruteForce();
        graded.FenwickBuckets();
        graded.IterationSetup();

        Assert.Equal(expectedScan, AnswerText.Of(graded.BruteForce()));
        Assert.Equal(expectedBuckets, AnswerText.Of(graded.FenwickBuckets()));
    }

    [Fact]
    public void BruteForce_AgreesWithFenwickBuckets()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FenwickBuckets()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void FenwickBuckets_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.FenwickBuckets()));
    }

    private static NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks BuildHarness()
    {
        var harness = new NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();
        harness.IterationSetup();

        return harness;
    }
}
