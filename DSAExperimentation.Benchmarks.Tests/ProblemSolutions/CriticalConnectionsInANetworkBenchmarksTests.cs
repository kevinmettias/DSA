using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CriticalConnectionsInANetworkBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - removing each connection and re-running a full
// connectivity search against this repo's low-link bridge search - so a harness whose arms disagree is
// timing two different problems. [GlobalSetup] tiles the node ids into triangles and chains each
// triangle to the next through a single linking edge, so a triangle's own three connections all sit on
// a cycle and are never critical while every linking edge is a bridge; the critical count is therefore
// decisive, and the same NodeCount must rebuild the same network.
public sealed partial class CriticalConnectionsInANetworkBenchmarksTests
{
    private const int SmallestNodeCount = 150;

    // [GlobalSetup] tiles the ids in groups of three and links consecutive groups.
    private const int TriangleSize = 3;

    // The only connections whose removal can disconnect the chain of triangles are the links between
    // consecutive ones, of which there is exactly one per group beyond the first.
    private const int ExpectedCriticalConnectionCount = (SmallestNodeCount / TriangleSize) - 1;

    // A triangle's own connections sit on a cycle, so at worst only the links between triangles count.
    private const int MinimumCriticalConnectionCount = 0;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().NaiveEdgeRemovalScan(),
            MinimumCriticalConnectionCount,
            ExpectedCriticalConnectionCount);
        Assert.Equal(BuildHarness().NaiveEdgeRemovalScan(), BuildHarness().NaiveEdgeRemovalScan());
    }

    [Fact]
    public void NaiveEdgeRemovalScan_ChainOfFiftyTriangles_AgreesWithLowLinkBridgeSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCriticalConnectionCount, harness.NaiveEdgeRemovalScan());
        Assert.Equal(harness.LowLinkBridgeSearch(), harness.NaiveEdgeRemovalScan());
    }

    [Fact]
    public void LowLinkBridgeSearch_ChainOfFiftyTriangles_AgreesWithNaiveEdgeRemovalScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCriticalConnectionCount, harness.LowLinkBridgeSearch());
        Assert.Equal(harness.NaiveEdgeRemovalScan(), harness.LowLinkBridgeSearch());
    }

    private static CriticalConnectionsInANetworkBenchmarks BuildHarness()
    {
        var harness = new CriticalConnectionsInANetworkBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
