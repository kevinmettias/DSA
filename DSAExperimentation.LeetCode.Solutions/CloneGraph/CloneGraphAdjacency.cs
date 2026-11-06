namespace DSAExperimentation.LeetCode.Solutions.CloneGraph;

// LC 133's graph as its judge writes it, an adjacency list: row i lists the values of node i + 1's neighbors,
// and the function is handed node 1. The judge reads the answer back the same way, and refuses a clone that
// shares a node with the graph it was given, which this reads back as no graph at all. Nodes are records
// compared by value, so sharing is asked of references.
internal static class CloneGraphAdjacency
{
    // Node 1 of the graph the rows describe, or null for LeetCode's empty graph [].
    public static Node? GraphOf(int[][] adjacencyList)
    {
        var nodes = new Node[adjacencyList.Length];

        for (var index = 0; index < nodes.Length; index++)
        {
            nodes[index] = new Node(index + 1);
        }

        for (var index = 0; index < nodes.Length; index++)
        {
            nodes[index].Neighbors.AddRange(adjacencyList[index].Select(neighbor => nodes[neighbor - 1]));
        }

        return nodes.FirstOrDefault();
    }

    // The graph reachable from the clone, as LeetCode lists it, or no rows when any node of it is one of the
    // given graph's: a clone that shares a node is no deep copy.
    public static int[][] AdjacencyOf(Node? clone, Node? graph)
    {
        var given = NodesFrom(graph);
        var cloned = NodesFrom(clone);

        if (cloned.Any(given.Contains))
        {
            return [];
        }

        return [.. cloned.OrderBy(node => node.Value).Select(node => node.Neighbors.Select(neighbor => neighbor.Value).ToArray())];
    }

    private static HashSet<Node> NodesFrom(Node? start)
    {
        var reached = new HashSet<Node>(ReferenceEqualityComparer.Instance);
        var pending = new Stack<Node>();

        if (start is not null && reached.Add(start))
        {
            pending.Push(start);
        }

        while (pending.TryPop(out var node))
        {
            foreach (var neighbor in node.Neighbors.Where(reached.Add))
            {
                pending.Push(neighbor);
            }
        }

        return reached;
    }
}
