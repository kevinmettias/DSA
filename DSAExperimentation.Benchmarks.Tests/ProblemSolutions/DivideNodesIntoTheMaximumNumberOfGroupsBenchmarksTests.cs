using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - hand-rolled adjacency-array walks
// against this repo's BipartiteCheck, KeyedDisjointSet and Reduce.Graph - so a harness whose arms
// disagree is timing two different problems. Setup builds one random spanning tree from a fixed
// seed, and a tree is always connected and always bipartite, so neither arm can reject the workload
// (the answer is never LC 2493's "no numbering exists" sentinel) and exactly one component is summed
// over, which is what keeps the result between the two groups any edge forces and the node count.
public sealed partial class DivideNodesIntoTheMaximumNumberOfGroupsBenchmarksTests
{
    private const int SmallestNodeCount = 50;
    private const int MinimumGroupsForAConnectedTreeWithEdges = 2;

    [Fact]
    public void Setup_RandomSpanningTree_LayersAboveOneGroupAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var groups = harness.ArrayAdjacencyBruteForce();

        Assert.InRange(groups, MinimumGroupsForAConnectedTreeWithEdges, SmallestNodeCount);
        Assert.Equal(groups, BuildHarness().ArrayAdjacencyBruteForce());
    }

    [Fact]
    public void ArrayAdjacencyBruteForce_BipartiteSpanningTree_AgreesWithReducePrimitivesComposition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReducePrimitivesComposition(), harness.ArrayAdjacencyBruteForce());
    }

    [Fact]
    public void ReducePrimitivesComposition_BipartiteSpanningTree_AgreesWithArrayAdjacencyBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayAdjacencyBruteForce(), harness.ReducePrimitivesComposition());
    }

    private static DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks BuildHarness()
    {
        var harness = new DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
