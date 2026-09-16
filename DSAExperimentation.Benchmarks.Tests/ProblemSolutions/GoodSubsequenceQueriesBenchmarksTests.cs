using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GoodSubsequenceQueriesBenchmarks (ARCHITECTURE 17.9): both arms are
// GoodSubsequenceQueriesSolution's - the per-query O(n) rescan against two persistent SegmentTrees
// point-updated in O(log n) - so a harness whose arms disagree is timing two different questions.
// This is one of the stateful classes where arm order has to be checked rather than assumed: the
// composed arm mutates the GoodSubsequenceIndex it is handed, but the two arms keep their state in
// separate fields (the baseline is handed its own _bruteForceNums, the composed arm its own _index),
// so one harness can be called once per arm in either order - and a second call of the composed arm
// without an intervening [IterationSetup] would replay the query stream against an index already
// carried forward from the first run. The IterationSetup test below pins exactly that down: a
// re-prepared harness must answer as a fresh one does. Setup builds nums and the query stream, so
// the same Length must rebuild both.
public sealed partial class GoodSubsequenceQueriesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

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

    [Fact]
    public void BruteForce_SeededNumsAndQueries_AgreesWithSegmentTreeGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeGcd(), harness.BruteForce());
    }

    [Fact]
    public void SegmentTreeGcd_SeededNumsAndQueries_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeGcd());
    }

    private static GoodSubsequenceQueriesBenchmarks BuildHarness()
    {
        var harness = new GoodSubsequenceQueriesBenchmarks { Length = SmallestLength };
        harness.Setup();
        harness.IterationSetup();

        return harness;
    }
}
