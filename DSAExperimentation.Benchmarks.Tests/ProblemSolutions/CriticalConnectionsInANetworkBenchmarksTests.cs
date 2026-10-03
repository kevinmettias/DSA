using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CriticalConnectionsInANetworkBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests
// cannot pin: which connections are critical, known from Setup's construction rather than from either arm.
// [GlobalSetup] tiles the node ids into triangles and chains each triangle to the next through a single linking
// edge, so a triangle's own three connections all sit on a cycle and are never critical while every linking edge
// is a bridge. LC 1192 leaves the answer's order free, so each connection is read as an unordered pair and the
// pairs are compared in sorted order.
public sealed partial class CriticalConnectionsInANetworkBenchmarksTests
{
    private const int SmallestNodeCount = 150;

    // [GlobalSetup] tiles the ids in groups of three and links consecutive groups.
    private const int TriangleSize = 3;

    // The only connections whose removal can disconnect the chain of triangles are the links between
    // consecutive ones, of which there is exactly one per group beyond the first.
    private const int ExpectedCriticalConnectionCount = (SmallestNodeCount / TriangleSize) - 1;

    [Fact]
    public void NaiveEdgeRemovalScan_ChainOfFiftyTriangles_FindsExactlyTheLinksBetweenTriangles() =>
        AssertFindsExactlyTheLinksBetweenTriangles(BuildHarness().NaiveEdgeRemovalScan());

    [Fact]
    public void LowLinkBridgeSearch_ChainOfFiftyTriangles_FindsExactlyTheLinksBetweenTriangles() =>
        AssertFindsExactlyTheLinksBetweenTriangles(BuildHarness().LowLinkBridgeSearch());

    private static void AssertFindsExactlyTheLinksBetweenTriangles(int[][] critical)
    {
        var links = Enumerable.Range(1, ExpectedCriticalConnectionCount)
            .Select(group => (Low: (group * TriangleSize) - 1, High: group * TriangleSize));
        var found = critical
            .Select(connection => (Low: connection.Min(), High: connection.Max()))
            .Order();

        Assert.Equal(ExpectedCriticalConnectionCount, critical.Length);
        Assert.Equal(links, found);
    }

    private static CriticalConnectionsInANetworkBenchmarks BuildHarness()
    {
        var harness = new CriticalConnectionsInANetworkBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
