namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds the worst case for both a naive per-start walk and Tarjan's SCC pass: one
// single cycle spanning every node (edges[i] = (i + 1) % n). Every start node's naive
// forward walk has to traverse the entire cycle before it revisits a node, so nothing
// short-circuits early - the same "force the real worst case" reasoning
// TwoSumBenchmarks' unreachable target already uses.
internal static class FunctionalGraphs
{
    public static int[] BuildSingleCycleEdges(int nodeCount)
    {
        var edges = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            edges[i] = (i + 1) % nodeCount;
        }

        return edges;
    }

    public static FunctionalGraphNode[] BuildNodes(int[] edges)
    {
        var nodes = new FunctionalGraphNode[edges.Length];

        for (var i = 0; i < edges.Length; i++)
        {
            nodes[i] = new FunctionalGraphNode(i);
        }

        for (var i = 0; i < edges.Length; i++)
        {
            nodes[i].Successors.Add(nodes[edges[i]]);
        }

        return nodes;
    }
}
