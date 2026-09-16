using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for EvenNodeSumGraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3910's tiny labeled graph being connected with a real cycle in it, so both connectivity
// strategies walk an actual graph instead of a bare spanning tree or a partitioned one.
public sealed partial class EvenNodeSumGraphWorkloadsTests
{
    private const int NodeCount = 13; // LC 3910's own upper bound
    private const int Seed = 3910; // LC problem number
    private const int NodeValueOff = 0;
    private const int NodeValueOn = 1;

    [Fact]
    public void Build_NodeCount_ReturnsOneValuePerNodeAndOneEdgeRowOnThoseNodes()
    {
        var (nums, edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount, nums.Length);
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
    }

    [Fact]
    public void Build_EveryValue_IsZeroOrOne() =>
        Assert.All(
            EvenNodeSumGraphWorkloads.Build(NodeCount, Seed).Nums,
            value => Assert.InRange(value, NodeValueOff, NodeValueOn));

    // The generator normalizes each pair before it goes into the set, so writing the lower
    // endpoint first is what makes a later (high, low) draw of an edge already stored collide
    // with it - the dedup the deliberately sparse edge count depends on.
    [Fact]
    public void Build_EveryEdge_WritesTheLowerEndpointFirst()
    {
        var (_, edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
    }

    [Fact]
    public void Build_EveryEdge_AppearsOnlyOnce()
    {
        var (_, edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);

        Assert.Equal(edges.Length, edges.Select(EdgeKey).Distinct().Count());
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_ReachesAnEarlierNode()
    {
        var (_, edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameGraph()
    {
        var (nums, edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);
        var (repeatNums, repeatEdges) = EvenNodeSumGraphWorkloads.Build(NodeCount, Seed);

        Assert.Equal(nums, repeatNums);
        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
    }

    private static (int Low, int High) EdgeKey(int[] edge) => (edge[0], edge[1]);
}
