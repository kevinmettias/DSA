using DSAExperimentation.LeetCode.PathWithMaximumProbability;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMaximumProbability;

// Harness only. The graph representation is PathWithMaximumProbabilitySolution's own
// ProbabilityGraph/ProbabilityNode/ProbabilityTopology and both strategies are its
// methods - this file pins them to LeetCode's three published examples plus two the
// original test did not cover: an edge that has to be traversed backwards (which only
// an honestly undirected graph finds) and a certain-probability chain.
public sealed class PathWithMaximumProbabilityTests
{
    private const int Precision = 5;

    public static TheoryData<MaxProbabilityCase> Examples =>
        new()
        {
            // LC example 1: 0 -> 1 -> 2 at 0.5 * 0.5 beats the direct 0.2 edge.
            { new MaxProbabilityCase(NodeCount: 3, Edges: [[0, 1], [1, 2], [0, 2]], SuccessProbabilities: [0.5, 0.5, 0.2], Start: 0, End: 2, Expected: 0.25) },

            // LC example 2: the direct 0.3 edge beats 0.5 * 0.5 = 0.25.
            { new MaxProbabilityCase(NodeCount: 3, Edges: [[0, 1], [1, 2], [0, 2]], SuccessProbabilities: [0.5, 0.5, 0.3], Start: 0, End: 2, Expected: 0.3) },

            // LC example 3: no path at all.
            { new MaxProbabilityCase(NodeCount: 3, Edges: [[0, 1]], SuccessProbabilities: [0.5], Start: 0, End: 2, Expected: 0.0) },

            // Edges stated low-to-high but walked high-to-low: 2 -> 1 -> 0.
            { new MaxProbabilityCase(NodeCount: 3, Edges: [[0, 1], [1, 2]], SuccessProbabilities: [0.5, 0.4], Start: 2, End: 0, Expected: 0.2) },

            // A chain of certainties stays certain.
            { new MaxProbabilityCase(NodeCount: 4, Edges: [[0, 1], [1, 2], [2, 3]], SuccessProbabilities: [1.0, 1.0, 1.0], Start: 0, End: 3, Expected: 1.0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProbabilityByExhaustiveDfs_LeetCodeExamples_ReturnsBestPathProduct(
        MaxProbabilityCase example)
    {
        var actual = PathWithMaximumProbabilitySolution.MaxProbabilityByExhaustiveDfs(
            example.NodeCount, example.Edges, example.SuccessProbabilities, (example.Start, example.End));

        Assert.Equal(example.Expected, actual, Precision);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProbabilityByDijkstra_LeetCodeExamples_ReturnsBestPathProduct(
        MaxProbabilityCase example)
    {
        var actual = PathWithMaximumProbabilitySolution.MaxProbabilityByDijkstra(
            example.NodeCount, example.Edges, example.SuccessProbabilities, (example.Start, example.End));

        Assert.Equal(example.Expected, actual, Precision);
    }

    // One LeetCode example: the node count, the undirected edges as [a, b] pairs with
    // their per-edge success probabilities, the two endpoints to connect, and the best
    // path product between them (0.0 when no path exists). Start and End are both bare
    // ints, so each is named at every construction site and a row reads as the case it
    // is rather than as two positions a caller has to keep in order. Nested because it
    // is only ever used inside this test class - it is this harness's own vocabulary,
    // not a type another file would import.
    public readonly record struct MaxProbabilityCase(
        int NodeCount, int[][] Edges, double[] SuccessProbabilities, int Start, int End, double Expected);
}
