using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignGraphWithShortestPathCalculatorBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the textbook O(V^2) per-query Dijkstra
// against this repo's own heap-frontier Dijkstra - so a harness whose arms disagree is timing two
// different problems. Setup builds both graphs and the query list from one fixed seed, so the same
// NodeCount must rebuild the same workload and hand both arms the identical graph.
public sealed partial class DesignGraphWithShortestPathCalculatorBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    // Mirrors the fixture's own query count and weight ceiling: every reported distance is a
    // simple path over edges of at most MaxEdgeWeight, so the summed report is bounded by them.
    private const int QueryCount = 200;
    private const int MaxEdgeWeight = 49;

    // Every query reports either a distance inside this band or the unreachable sentinel.
    private const long MinimumTotalDistance = -QueryCount;

    private const long MaximumTotalDistance = (long)QueryCount * (SmallestNodeCount - 1) * MaxEdgeWeight;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameGraphAndQueries()
    {
        var total = BuildHarness().ArrayDijkstra();

        // A query reports either a real shortest-path distance - a simple path of at most
        // NodeCount - 1 edges, none of them heavier than MaxEdgeWeight - or the unreachable
        // sentinel, so the summed report of QueryCount queries has to sit inside that band. The
        // fixture exposes nothing else about the workload through the public surface; the same
        // NodeCount rebuilding the same band is what makes two published numbers comparable.
        Assert.InRange(total, MinimumTotalDistance, MaximumTotalDistance);
        Assert.Equal(total, BuildHarness().ArrayDijkstra());
    }

    [Fact]
    public void ArrayDijkstra_SameGraphAsHeapDijkstra_AgreesWithHeapDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapDijkstra(), harness.ArrayDijkstra());
    }

    [Fact]
    public void HeapDijkstra_SameGraphAsArrayDijkstra_AgreesWithArrayDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayDijkstra(), harness.HeapDijkstra());
    }

    private static DesignGraphWithShortestPathCalculatorBenchmarks BuildHarness()
    {
        var harness = new DesignGraphWithShortestPathCalculatorBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
