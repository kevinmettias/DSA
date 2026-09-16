using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathWithAtMostKConsecutiveIdenticalCharactersBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - a BCL
// priority queue over the raw edges against a shortest-path search over the prepared
// (node, runLength) state graph - so a harness whose arms disagree is searching two different
// graphs. Setup builds both the edge/label workload and the state graph from one fixed seed,
// so the same NodeCount must rebuild the same workload; otherwise two published numbers were
// never comparable.
public sealed partial class ShortestPathWithAtMostKConsecutiveIdenticalCharactersBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BclPriorityQueue(), BuildHarness().BclPriorityQueue());

    [Fact]
    public void BclPriorityQueue_HundredNodeRunGraph_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraph(), harness.BclPriorityQueue());
    }

    [Fact]
    public void ReduceGraph_HundredNodeRunGraph_AgreesWithBclPriorityQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclPriorityQueue(), harness.ReduceGraph());
    }

    private static ShortestPathWithAtMostKConsecutiveIdenticalCharactersBenchmarks BuildHarness()
    {
        var harness =
            new ShortestPathWithAtMostKConsecutiveIdenticalCharactersBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
