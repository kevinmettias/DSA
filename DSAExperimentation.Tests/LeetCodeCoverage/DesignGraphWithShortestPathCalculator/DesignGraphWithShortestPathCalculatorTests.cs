using DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignGraphWithShortestPathCalculator;

// Harness only: both strategies live in
// DesignGraphWithShortestPathCalculatorSolution, and so does the graph
// representation each one searches. LeetCode's own shape here is a stateful
// object across a sequence of calls, so Examples encodes a call script instead of
// a single argument tuple - the same shape DesignBrowserHistoryTests uses for its
// own instance-API problem. An addEdge returns null in LeetCode's judge output,
// so ShortestPathGraphOp.Apply returns null for it too and the expected sequence
// reads exactly like the published one.
public sealed class DesignGraphWithShortestPathCalculatorTests
{
    public static TheoryData<int, int[][], ShortestPathGraphOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example: node 3 reaches node 2 the long way
                // round, node 3 is unreachable from node 0 until addEdge creates a
                // route into it.
                4,
                [[0, 2, 5], [0, 1, 2], [1, 2, 1], [3, 0, 3]],
                [
                    ShortestPathGraphOp.ShortestPath(3, 2),
                    ShortestPathGraphOp.ShortestPath(0, 3),
                    ShortestPathGraphOp.AddEdge([1, 3, 4]),
                    ShortestPathGraphOp.ShortestPath(0, 3),
                ],
                [6, -1, null, 6]
            },
            {
                // Every added edge has to be visible to the very next query, both
                // when it opens a route that did not exist and when it undercuts
                // one that did.
                4,
                [[0, 2, 5]],
                [
                    ShortestPathGraphOp.ShortestPath(0, 2),
                    ShortestPathGraphOp.ShortestPath(0, 3),
                    ShortestPathGraphOp.AddEdge([2, 3, 2]),
                    ShortestPathGraphOp.ShortestPath(0, 3),
                    ShortestPathGraphOp.AddEdge([0, 1, 1]),
                    ShortestPathGraphOp.AddEdge([1, 2, 1]),
                    ShortestPathGraphOp.ShortestPath(0, 2),
                    ShortestPathGraphOp.ShortestPath(0, 3),
                ],
                [5, -1, null, 7, null, null, 2, 4]
            },
            {
                // No edges at all: a node still reaches itself at cost 0, and
                // nothing else.
                2,
                [],
                [
                    ShortestPathGraphOp.ShortestPath(0, 0),
                    ShortestPathGraphOp.ShortestPath(0, 1),
                ],
                [0, -1]
            },
            {
                // Edges are directed, so reachability is not symmetric, and a node
                // with no outgoing edge at all reaches only itself.
                3,
                [[1, 0, 4]],
                [
                    ShortestPathGraphOp.ShortestPath(1, 0),
                    ShortestPathGraphOp.ShortestPath(0, 1),
                    ShortestPathGraphOp.ShortestPath(2, 2),
                ],
                [4, -1, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathGraphByArrayDijkstra_LeetCodeExamples_MatchesExpectedSequence(
        int n, int[][] edges, ShortestPathGraphOp[] operations, int?[] expected) =>
        RunScript(
            new DesignGraphWithShortestPathCalculatorSolution.ShortestPathGraphByArrayDijkstra(n, edges),
            operations,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathGraphByHeapDijkstra_LeetCodeExamples_MatchesExpectedSequence(
        int n, int[][] edges, ShortestPathGraphOp[] operations, int?[] expected) =>
        RunScript(
            new DesignGraphWithShortestPathCalculatorSolution.ShortestPathGraphByHeapDijkstra(n, edges),
            operations,
            expected);

    private static void RunScript(
        DesignGraphWithShortestPathCalculatorSolution.IShortestPathGraph graph,
        ShortestPathGraphOp[] operations,
        int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(graph));
        }
    }
}
