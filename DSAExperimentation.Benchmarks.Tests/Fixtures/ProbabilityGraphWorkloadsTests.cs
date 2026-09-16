using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ProbabilityGraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC 1514's
// graph being stated as forward-only edge pairs with probabilities high enough that long paths stay
// competitive with short ones, so the exhaustive-path baseline cannot be won by a trivially dominant
// single edge.
public sealed partial class ProbabilityGraphWorkloadsTests
{
    private const int NodeCount = 10;
    private const int ExtraEdgesPerNode = 2;
    private const int Seed = 1514; // LC problem number
    private const int EdgeFieldCount = 2;
    private const double MinEdgeProbability = 0.5;
    private const double MaxEdgeProbability = MinEdgeProbability + 0.49;
    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void Build_EdgesAndProbabilities_ReturnOneProbabilityPerEdge()
    {
        var (edges, probabilities) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Equal(edges.Length, probabilities.Length);
    }

    [Fact]
    public void Build_EveryEdge_PointsForwardWithAProbabilityInsideTheDocumentedBand()
    {
        var (edges, probabilities) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
        Assert.All(
            probabilities,
            probability => Assert.InRange(
                probability,
                MinEdgeProbability - RelativeTolerance,
                MaxEdgeProbability + RelativeTolerance));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var (edges, _) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var (edges, _) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(
            edges.Length,
            NodeCount - 1,
            (NodeCount - 1) * (1 + ExtraEdgesPerNode));
    }

    // The probabilities are doubles, so the repeat is compared inside a named tolerance rather than by
    // exact equality - the same reason the band above carries one.
    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, probabilities) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);
        var (repeatEdges, repeatProbabilities) =
            ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(probabilities.Length, repeatProbabilities.Length);
        Assert.All(
            Enumerable.Range(0, probabilities.Length),
            index => Assert.InRange(
                repeatProbabilities[index],
                probabilities[index] - RelativeTolerance,
                probabilities[index] + RelativeTolerance));
    }
}
