using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeSpanningTreeStabilityWithUpgradesBenchmarks (ARCHITECTURE 17.9): both
// arms are competing strategies for one question - the best spanning-tree stability reachable with
// the given upgrade budget - so a harness whose arms disagree is timing two different problems.
// Setup draws the edge list and the budget from the fixture's fixed seed, so the same node count
// must rebuild the same graph; the arms read that one prebuilt StabilityGraph and mutate nothing.
public sealed partial class MaximizeSpanningTreeStabilityWithUpgradesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArrayUnionFind(), BuildHarness().ArrayUnionFind());

    [Fact]
    public void ArrayUnionFind_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayUnionFind(), harness.DisjointSet());
    }

    [Fact]
    public void DisjointSet_AgreesWithArrayUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSet(), harness.ArrayUnionFind());
    }

    private static MaximizeSpanningTreeStabilityWithUpgradesBenchmarks BuildHarness()
    {
        var harness = new MaximizeSpanningTreeStabilityWithUpgradesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
