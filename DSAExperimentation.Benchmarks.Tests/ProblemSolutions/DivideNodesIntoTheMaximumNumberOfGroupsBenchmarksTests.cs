using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks (ARCHITECTURE 17.9): a
// bound the workload's shape fixes without either arm. Setup builds one random spanning tree from a
// fixed seed, and a tree is always connected and always bipartite, so the workload is never rejected
// (the answer is never LC 2493's "no numbering exists" sentinel) and exactly one component is summed
// over, which is what keeps the result between the two groups any edge forces and the node count.
public sealed partial class DivideNodesIntoTheMaximumNumberOfGroupsBenchmarksTests
{
    private const int SmallestNodeCount = 50;
    private const int MinimumGroupsForAConnectedTreeWithEdges = 2;

    [Fact]
    public void ArrayAdjacencyBruteForce_RandomSpanningTree_LayersAboveOneGroup() =>
        Assert.InRange(
            BuildHarness().ArrayAdjacencyBruteForce(),
            MinimumGroupsForAConnectedTreeWithEdges,
            SmallestNodeCount);

    private static DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks BuildHarness()
    {
        var harness = new DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
