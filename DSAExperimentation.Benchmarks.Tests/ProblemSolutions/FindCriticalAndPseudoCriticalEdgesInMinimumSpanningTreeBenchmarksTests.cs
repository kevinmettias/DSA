using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks (ARCHITECTURE 17.9), for
// what BenchmarkArmsTests cannot pin: bounds on how many edges get classified, which follow from Setup's
// construction rather than from either arm. Both arms return the two classified index lists. Setup's spanning chain
// makes the graph connected, so an MST exists and every one of its edges lands in one of the two lists; no more
// edges can be classified than exist.
public sealed partial class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarksTests
{
    private const int SmallestNodeCount = 40;
    private const int ExtraEdgeMultiplier = 3;

    // Every edge of a minimum spanning tree is critical or pseudo-critical, and a connected graph's
    // tree has one fewer edge than it has nodes.
    private const int MinClassifiedEdgeCount = SmallestNodeCount - 1;

    // Setup adds a spanning chain plus extra random edges, dropping only the self-loops it draws.
    private const int MaxClassifiedEdgeCount =
        (SmallestNodeCount - 1) + (SmallestNodeCount * ExtraEdgeMultiplier);

    [Fact]
    public void NaiveBfsConnectivity_SpanningChainPlusRandomEdges_ClassifiesAtLeastAnMstOfEdges()
    {
        var (critical, pseudoCritical) = BuildHarness().NaiveBfsConnectivity();

        Assert.InRange(critical.Length + pseudoCritical.Length, MinClassifiedEdgeCount, MaxClassifiedEdgeCount);
    }

    [Fact]
    public void DisjointSetKruskal_SpanningChainPlusRandomEdges_ClassifiesAtLeastAnMstOfEdges()
    {
        var (critical, pseudoCritical) = BuildHarness().DisjointSetKruskal();

        Assert.InRange(critical.Length + pseudoCritical.Length, MinClassifiedEdgeCount, MaxClassifiedEdgeCount);
    }

    private static FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks BuildHarness()
    {
        var harness = new FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
