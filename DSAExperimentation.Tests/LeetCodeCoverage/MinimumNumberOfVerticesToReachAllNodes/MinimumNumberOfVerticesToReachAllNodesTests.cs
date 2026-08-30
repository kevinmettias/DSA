using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVerticesToReachAllNodes;

// LeetCode 1557. Minimum Number of Vertices to Reach All Nodes: in a DAG, the unique
// minimal set that reaches every node is exactly the nodes with in-degree zero - any
// node with an incoming edge is already reachable from its source, and a source-less
// node can never be reached from anywhere else. One pass over the edges into this
// repo's own Set<int> (HashMap<Element,bool>-backed, per Set.cs's own doc comment)
// marks every node that has an incoming edge; the answer is everything left over.
public sealed partial class MinimumNumberOfVerticesToReachAllNodesTests
{
    [Fact]
    public void FindSmallestSetOfVertices_ClassicExample_ReturnsBothSourceNodes()
    {
        int[][] edges = [[0, 1], [0, 2], [2, 5], [3, 4], [4, 2]];

        var result = FindSmallestSetOfVertices(6, edges);

        Assert.Equal([0, 3], result);
    }

    [Fact]
    public void FindSmallestSetOfVertices_NoEdges_ReturnsEveryNode()
    {
        var result = FindSmallestSetOfVertices(3, []);

        Assert.Equal([0, 1, 2], result);
    }

    private static List<int> FindSmallestSetOfVertices(int n, int[][] edges)
    {
        var hasIncomingEdge = new Set<int>();
        foreach (var edge in edges)
        {
            hasIncomingEdge.TryAdd(edge[1]);
        }

        var result = new List<int>();
        for (var node = 0; node < n; node++)
        {
            if (!hasIncomingEdge.Has(node))
            {
                result.Add(node);
            }
        }

        return result;
    }
}
