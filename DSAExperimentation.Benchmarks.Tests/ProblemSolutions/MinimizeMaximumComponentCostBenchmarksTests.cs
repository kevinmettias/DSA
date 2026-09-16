using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeMaximumComponentCostBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the union-find baseline that sorts the edge list itself
// against Kruskal over a pre-built ComponentGraph - so a harness whose arms disagree is timing two
// different problems. Both arms return the minimum achievable largest component cost as an int, so they
// are compared directly; the two are handed the same edges in two different shapes (LeetCode's own
// (nodeCount, edges, maxComponents) triple against the prepared topology), so agreeing on the number also
// pins that the prepared topology really is that edge list. Neither arm mutates the edges, so one harness
// is safe to read twice in either order, and Setup builds both shapes from one fixed seed, so the same
// NodeCount must rebuild the same graph.
public sealed partial class MinimizeMaximumComponentCostBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnionFindSort(), BuildHarness().UnionFindSort());

    [Fact]
    public void UnionFindSort_SeededWeightedGraph_AgreesWithKruskalMst()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KruskalMst(), harness.UnionFindSort());
    }

    [Fact]
    public void KruskalMst_SeededWeightedGraph_AgreesWithUnionFindSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnionFindSort(), harness.KruskalMst());
    }

    private static MinimizeMaximumComponentCostBenchmarks BuildHarness()
    {
        var harness = new MinimizeMaximumComponentCostBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
