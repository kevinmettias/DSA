using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks (ARCHITECTURE 17.9): one
// harness reused across runs, as BenchmarkDotNet reuses it and the generic check, which builds a fresh
// harness per run, never does. This is one of the stateful classes where the re-preparation has to be
// checked rather than assumed: the composed arm MUTATES the index it is handed, so a second call
// without an intervening [IterationSetup] would replay the queries against an index carried forward
// from the first run - the IterationSetup test below pins exactly that down.
public sealed partial class NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarksTests
{
    private const int SmallestElementCount = 200;

    [Fact]
    public void IterationSetup_AfterAGradedRun_RePresentsThePristineWorkload()
    {
        var reference = BuildHarness();
        var graded = BuildHarness();
        var expectedScan = AnswerGraphText.Of(reference.BruteForce());
        var expectedBuckets = AnswerGraphText.Of(reference.FenwickBuckets());

        graded.BruteForce();
        graded.FenwickBuckets();
        graded.IterationSetup();

        Assert.Equal(expectedScan, AnswerGraphText.Of(graded.BruteForce()));
        Assert.Equal(expectedBuckets, AnswerGraphText.Of(graded.FenwickBuckets()));
    }

    private static NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks BuildHarness()
    {
        var harness = new NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();
        harness.IterationSetup();

        return harness;
    }
}
