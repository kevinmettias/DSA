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

    public static TheoryData<int, int[][], double[], int, int, double> Examples =>
        new()
        {
            // LC example 1: 0 -> 1 -> 2 at 0.5 * 0.5 beats the direct 0.2 edge.
            { 3, [[0, 1], [1, 2], [0, 2]], [0.5, 0.5, 0.2], 0, 2, 0.25 },

            // LC example 2: the direct 0.3 edge beats 0.5 * 0.5 = 0.25.
            { 3, [[0, 1], [1, 2], [0, 2]], [0.5, 0.5, 0.3], 0, 2, 0.3 },

            // LC example 3: no path at all.
            { 3, [[0, 1]], [0.5], 0, 2, 0.0 },

            // Edges stated low-to-high but walked high-to-low: 2 -> 1 -> 0.
            { 3, [[0, 1], [1, 2]], [0.5, 0.4], 2, 0, 0.2 },

            // A chain of certainties stays certain.
            { 4, [[0, 1], [1, 2], [2, 3]], [1.0, 1.0, 1.0], 0, 3, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProbabilityByExhaustiveDfs_LeetCodeExamples_ReturnsBestPathProduct(
        int nodeCount, int[][] edges, double[] successProbabilities, int start, int end, double expected) =>
        Assert.Equal(
            expected,
            PathWithMaximumProbabilitySolution.MaxProbabilityByExhaustiveDfs(
                nodeCount, edges, successProbabilities, (start, end)),
            Precision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProbabilityByDijkstra_LeetCodeExamples_ReturnsBestPathProduct(
        int nodeCount, int[][] edges, double[] successProbabilities, int start, int end, double expected) =>
        Assert.Equal(
            expected,
            PathWithMaximumProbabilitySolution.MaxProbabilityByDijkstra(
                nodeCount, edges, successProbabilities, (start, end)),
            Precision);
}
