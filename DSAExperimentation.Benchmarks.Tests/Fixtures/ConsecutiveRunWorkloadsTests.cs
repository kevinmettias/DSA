using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ConsecutiveRunWorkloads (ARCHITECTURE 17.7). The reading depends on the
// graph being forward-only and every node past the first reaching an earlier one - which is what
// keeps node n-1 reachable from node 0 - and on the labels cycling a small alphabet so crossings
// genuinely extend or reset a run instead of trivially always resetting it.
public sealed partial class ConsecutiveRunWorkloadsTests
{
    private const int NodeCount = 16;
    private const int Seed = 3970; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int WeightFieldIndex = 2;
    private const int MinWeight = 1;
    private const int MaxWeight = 9_999;
    private const string Alphabet = "abc";

    [Fact]
    public void Build_EveryEdge_PointsForwardWithAWeightInsideTheBand()
    {
        var (edges, _) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[WeightFieldIndex], MinWeight, MaxWeight));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_GetsAnEdgeFromAnEarlierNode()
    {
        var (edges, _) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_Labels_GiveOneAlphabetCharacterPerNode()
    {
        var (_, labels) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount, labels.Length);
        Assert.All(labels, label => Assert.True(Alphabet.Contains(label)));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, labels) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatLabels) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(labels, repeatLabels);
    }
}
