using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumNumberOfVerticesToReachAllNodes;

// LeetCode 1557. Minimum Number of Vertices to Reach All Nodes: in a DAG, the unique
// minimal set that reaches every node is exactly the nodes with in-degree zero - any
// node with an incoming edge is already reachable from its source, and a source-less
// node can never be reached from anywhere else.
//
// So neither strategy searches the graph at all; they differ only in how they answer
// "does this node have an incoming edge" - by rescanning the edge list once per node,
// or by marking every edge target up front.
internal static class MinimumNumberOfVerticesToReachAllNodesSolution
{
    // Indices into LeetCode's own [from, to] edge pair.
    private const int EdgeTo = 1;

    // The textbook answer: for every node, rescan every edge looking for one that
    // points at it. Deliberately written without this repo's primitives - O(V*E) with
    // no auxiliary structure at all - it is the arm the marking pass below has to
    // justify itself against.
    public static List<int> FindSmallestSetOfVerticesByNestedScan(int n, int[][] edges)
    {
        var sources = new List<int>();

        for (var node = 0; node < n; node++)
        {
            var hasIncomingEdge = false;

            foreach (var edge in edges)
            {
                if (edge[EdgeTo] == node)
                {
                    hasIncomingEdge = true;
                    break;
                }
            }

            if (!hasIncomingEdge)
            {
                sources.Add(node);
            }
        }

        return sources;
    }

    // One O(V+E) pass over the edges into this repo's own Set<int>
    // (HashMap<Element,bool>-backed, per Set.cs's own doc comment) marks every node
    // that has an incoming edge; the answer is everything left over, collected in
    // ascending id order because the node scan runs that way.
    public static List<int> FindSmallestSetOfVerticesByInDegreeSet(int n, int[][] edges)
    {
        var hasIncomingEdge = new Set<int>();

        foreach (var edge in edges)
        {
            hasIncomingEdge.TryAdd(edge[EdgeTo]);
        }

        var sources = new List<int>();

        for (var node = 0; node < n; node++)
        {
            if (!hasIncomingEdge.Has(node))
            {
                sources.Add(node);
            }
        }

        return sources;
    }
}
