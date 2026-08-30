using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathVisitingAllNodes.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathVisitingAllNodes;

// LeetCode 847. Shortest Path Visiting All Nodes: states are (current node, bitmask
// of nodes visited so far) pairs of an implicit graph over the input adjacency list -
// VisitStateNode/VisitStateTopology mirror OpenTheLockTests' LockNode/LockTopology
// exactly, just over (node, mask) states instead of 4-digit combinations. True
// multi-source BFS (every node starting simultaneously at depth 0) is equivalent to
// running Reduce.Graph's single-source BFS distance map from each node separately
// and taking the minimum, over all starts, of "the first depth at which any state
// reaches the full mask" - min-of-mins commutes, since every edge here has uniform
// cost 1, the same reasoning DistanceMapReduceAlgebra's own doc comment already
// relies on for a single target.
public sealed partial class ShortestPathVisitingAllNodesTests
{
    [Fact]
    public void ShortestPathLength_StarWithThreeLeaves_ReturnsFour()
    {
        int[][] graph = [[1, 2, 3], [0], [0], [0]];

        var length = ShortestPathLength(graph);

        Assert.Equal(4, length);
    }

    [Fact]
    public void ShortestPathLength_ThreeNodePathGraph_ReturnsTwo()
    {
        int[][] graph = [[1], [0, 2], [1]];

        var length = ShortestPathLength(graph);

        Assert.Equal(2, length);
    }

    private static int ShortestPathLength(int[][] graph)
    {
        var nodesByState = BuildStateGraph(graph);
        var fullMask = (1 << graph.Length) - 1;
        var shortest = int.MaxValue;

        for (var start = 0; start < graph.Length; start++)
        {
            var startNode = nodesByState[(start, 1 << start)];

            var distances = Reduce.Graph<
                VisitStateNode, VisitStateTopology, ListChildren<VisitStateNode>,
                NaturalChildOrder<VisitStateNode, ListChildren<VisitStateNode>>, ListChildren<VisitStateNode>,
                BreadthFirstReduceOrder<VisitStateNode>,
                DistanceMapReduceAlgebra<VisitStateNode>, Dictionary<VisitStateNode, int>>(startNode);

            foreach (var (state, distance) in distances)
            {
                if (state.Mask == fullMask && distance < shortest)
                {
                    shortest = distance;
                }
            }
        }

        return shortest;
    }

    // Every (node, visited-mask) pair becomes a state; mirrors OpenTheLockTests'
    // BuildGraph, materializing the whole combinatorial space up front rather than
    // discovering it lazily.
    private static Dictionary<(int Node, int Mask), VisitStateNode> BuildStateGraph(int[][] graph)
    {
        var stateCount = 1 << graph.Length;
        var nodesByState = new Dictionary<(int Node, int Mask), VisitStateNode>();

        for (var node = 0; node < graph.Length; node++)
        {
            for (var mask = 0; mask < stateCount; mask++)
            {
                nodesByState[(node, mask)] = new VisitStateNode(node, mask);
            }
        }

        foreach (var state in nodesByState.Values)
        {
            foreach (var neighbor in graph[state.Node])
            {
                state.Neighbors.Add(nodesByState[(neighbor, state.Mask | (1 << neighbor))]);
            }
        }

        return nodesByState;
    }
}
