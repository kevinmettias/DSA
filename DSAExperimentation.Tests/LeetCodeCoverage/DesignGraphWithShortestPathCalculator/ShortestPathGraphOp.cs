using DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignGraphWithShortestPathCalculator;

// One call in an LC 2642 script: either an edge to add or a pair of nodes to
// query. Pure dispatch, built via the named factories below so a script reads
// like the LeetCode call sequence it replays.
public readonly record struct ShortestPathGraphOp(ShortestPathGraphOp.OpKind kind, int[] edge, int node1, int node2)
{
    public static ShortestPathGraphOp AddEdge(int[] edge) => new(OpKind.AddEdge, edge, 0, 0);

    public static ShortestPathGraphOp ShortestPath(int node1, int node2) => new(OpKind.ShortestPath, [], node1, node2);

    // null for addEdge, matching LeetCode's own judge output for a void
    // operation; the query's answer otherwise - so a script runner can assert
    // against one expected value per operation uniformly.
    internal int? Apply(DesignGraphWithShortestPathCalculatorSolution.IShortestPathGraph graph)
    {
        if (kind == OpKind.AddEdge)
        {
            graph.AddEdge(edge);

            return null;
        }

        return graph.ShortestPathBetween(node1, node2);
    }

    public enum OpKind
    {
        AddEdge,
        ShortestPath,
    }
}
