using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NetworkRecoveryWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3620's
// graph being a DAG by construction whose forward edges alone reach the last node from the first, with
// an online/offline split that leaves a real recovery path to be found.
public sealed partial class NetworkRecoveryWorkloadsTests
{
    private const int NodeCount = 32;
    private const int ExtraEdgesPerNode = 2;
    private const int Seed = 3620; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Cost
    private const int MinCost = 0;
    private const int CostUpperBound = 1_000_000_000;
    private const int FirstNode = 0;
    private const int LastNode = NodeCount - 1;

    [Fact]
    public void Build_NodeCount_ReturnsOneOnlineFlagPerNode() =>
        Assert.Equal(NodeCount, NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed).Online.Length);

    [Fact]
    public void Build_EveryEdge_PointsForwardWithACostInsideTheDocumentedBand()
    {
        var (edges, _) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], FirstNode, LastNode));
        Assert.All(edges, edge => Assert.InRange(edge[1], FirstNode, LastNode));
        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinCost, CostUpperBound - 1));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var (edges, _) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_Online_KeepsTheFirstAndLastNodesOnline()
    {
        var (_, online) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.True(online[FirstNode]);
        Assert.True(online[LastNode]);
    }

    // The offline flag is a probabilistic draw, so what is asserted is the structural fact the reading
    // needs: some node is offline, which is what there is to recover around, and the rest are not.
    [Fact]
    public void Build_Online_LeavesBothFlagsPresent()
    {
        var (_, online) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Contains(online, flag => flag);
        Assert.Contains(online, flag => !flag);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, online) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);
        var (repeatEdges, repeatOnline) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(online, repeatOnline);
    }
}
