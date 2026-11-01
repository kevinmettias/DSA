using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignGraphWithShortestPathCalculatorBenchmarks (ARCHITECTURE 17.9): the
// generic arm check compares the arms only with each other, so what stays here is a band derived from
// the workload's construction rather than from either arm.
public sealed partial class DesignGraphWithShortestPathCalculatorBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    // Mirrors the fixture's own query count and weight ceiling: every reported distance is a
    // simple path over edges of at most MaxEdgeWeight, so the summed report is bounded by them.
    private const int QueryCount = 100;
    private const int MaxEdgeWeight = 49;

    // Every query reports either a distance inside this band or the unreachable sentinel.
    private const long MinimumTotalDistance = -QueryCount;

    private const long MaximumTotalDistance = (long)QueryCount * (SmallestNodeCount - 1) * MaxEdgeWeight;

    // A query reports either a real shortest-path distance - a simple path of at most NodeCount - 1
    // edges, none of them heavier than MaxEdgeWeight - or the unreachable sentinel, so the summed
    // report of QueryCount queries has to sit inside that band.
    [Fact]
    public void ArrayDijkstra_FiftyNodeGraph_SumsDistancesInsideTheSimplePathBand() =>
        Assert.InRange(BuildHarness().ArrayDijkstra(), MinimumTotalDistance, MaximumTotalDistance);

    private static DesignGraphWithShortestPathCalculatorBenchmarks BuildHarness()
    {
        var harness = new DesignGraphWithShortestPathCalculatorBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
