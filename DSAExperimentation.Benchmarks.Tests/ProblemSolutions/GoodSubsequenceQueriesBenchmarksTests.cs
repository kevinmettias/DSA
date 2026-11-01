using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GoodSubsequenceQueriesBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a freshly built workload, so this asserts only what that
// check never does - run an arm again on a harness BenchmarkDotNet reuses across iterations. Both
// arms mutate the state they are handed (the baseline its own _bruteForceNums, the composed arm the
// SegmentTrees of its own _index), so a second call without an intervening [IterationSetup] would
// replay the query stream against state already carried forward from the first run. The test below
// pins that down: a harness re-prepared after a graded run must answer as a fresh one does.
public sealed partial class GoodSubsequenceQueriesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void IterationSetup_AfterAGradedRun_RePresentsThePristineWorkload()
    {
        var reference = BuildHarness();
        var graded = BuildHarness();
        var expectedScan = reference.BruteForce();
        var expectedTree = reference.SegmentTreeGcd();

        graded.BruteForce();
        graded.SegmentTreeGcd();
        graded.IterationSetup();

        Assert.Equal(expectedScan, graded.BruteForce());
        Assert.Equal(expectedTree, graded.SegmentTreeGcd());
    }

    private static GoodSubsequenceQueriesBenchmarks BuildHarness()
    {
        var harness = new GoodSubsequenceQueriesBenchmarks { Length = SmallestLength };
        harness.Setup();
        harness.IterationSetup();

        return harness;
    }
}
