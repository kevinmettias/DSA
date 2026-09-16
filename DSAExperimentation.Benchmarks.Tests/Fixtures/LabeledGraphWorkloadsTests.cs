using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LabeledGraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3615's
// graph being connected on a small alphabet, so same-label pairs stay frequent and both strategies do
// real palindrome-growing work instead of bottoming out at length 1 immediately.
public sealed partial class LabeledGraphWorkloadsTests
{
    private const int NodeCount = 14; // LC 3615's own upper bound
    private const int ExtraEdgesPerNode = 2;
    private const int Seed = 3615; // LC problem number
    private const int EdgeFieldCount = 2; // FirstNode, SecondNode
    private const int AlphabetSize = 4;

    [Fact]
    public void Build_NodeCount_ReturnsOneLabelCharacterPerNode() =>
        Assert.Equal(NodeCount, LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed).Label.Length);

    [Fact]
    public void Build_EveryLabelCharacter_StaysOnTheSmallAlphabet()
    {
        var (_, label) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(label, character => Assert.InRange(character, 'a', (char)('a' + AlphabetSize - 1)));
    }

    [Fact]
    public void Build_EveryEdge_IsATwoNodePairWithinTheNodeRange()
    {
        var (edges, _) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
    }

    // The generator writes a node's guaranteed edge as { node, earlier node }, and it is that edge
    // which keeps the graph connected - the shape both strategies' walks rely on.
    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnEdgeToAnEarlierNode()
    {
        var (edges, _) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[0] == node && edge[1] < node);
        }
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameGraph()
    {
        var (edges, label) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);
        var (repeatEdges, repeatLabel) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(label, repeatLabel);
    }
}
