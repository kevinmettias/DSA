using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RedundantConnectionIIBenchmarks (ARCHITECTURE 17.9): both arms are
// RedundantConnectionIISolution's, competing strategies for the same question - validity retried
// from scratch for every candidate removal against the DisjointSet approach that settles it in at
// most two passes - so a harness whose arms disagree removes two different edges. Setup grows the
// chain 1->2->...->n plus a single extra edge back to node 2 from NodeCount alone, so the same
// NodeCount must rebuild the same edge list.
public sealed partial class RedundantConnectionIIBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FindRedundantEdgeByRemovalScan()),
            AnswerText.Of(BuildHarness().FindRedundantEdgeByRemovalScan()));

    [Fact]
    public void FindRedundantEdgeByRemovalScan_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.FindRedundantEdgeByDisjointSet()),
            AnswerText.Of(harness.FindRedundantEdgeByRemovalScan()));
    }

    [Fact]
    public void FindRedundantEdgeByDisjointSet_AgreesWithRemovalScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.FindRedundantEdgeByRemovalScan()),
            AnswerText.Of(harness.FindRedundantEdgeByDisjointSet()));
    }

    private static RedundantConnectionIIBenchmarks BuildHarness()
    {
        var harness = new RedundantConnectionIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
