using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite.Fixtures;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite;

// LeetCode 785. Is Graph Bipartite?: exactly this repo's own BipartiteCheck -
// multi-root BFS 2-coloring over the graph's own (already-symmetric) adjacency
// list, returning false the moment an edge connects two same-colored nodes.
public sealed partial class IsGraphBipartiteTests
{
    [Fact]
    public void IsBipartite_ClassicOddCycleThroughNodeZero_ReturnsFalse()
    {
        // graph = [[1,2,3],[0,2],[0,1,3],[0,2]] - 1-2 and 0's other edges force an
        // odd cycle (0-1-2-0).
        var nodes = BuildGraph([[1, 2, 3], [0, 2], [0, 1, 3], [0, 2]]);

        var result = Check(nodes);

        Assert.False(result);
    }

    [Fact]
    public void IsBipartite_EvenCycle_ReturnsTrue()
    {
        // graph = [[1,3],[0,2],[1,3],[0,2]] - a clean 4-cycle, 2-colorable.
        var nodes = BuildGraph([[1, 3], [0, 2], [1, 3], [0, 2]]);

        var result = Check(nodes);

        Assert.True(result);
    }

    private static List<GraphNode> BuildGraph(int[][] adjacency)
    {
        var nodes = Enumerable.Range(0, adjacency.Length).Select(id => new GraphNode(id)).ToList();

        for (var i = 0; i < adjacency.Length; i++)
        {
            foreach (var neighbor in adjacency[i])
            {
                nodes[i].Neighbors.Add(nodes[neighbor]);
            }
        }

        return nodes;
    }

    private static bool Check(List<GraphNode> nodes)
        => BipartiteCheckOperations.IsBipartite<
            GraphNode, GraphNodeTopology, ListChildren<GraphNode>,
            NaturalChildOrder<GraphNode, ListChildren<GraphNode>>, ListChildren<GraphNode>>(
            nodes);
}
