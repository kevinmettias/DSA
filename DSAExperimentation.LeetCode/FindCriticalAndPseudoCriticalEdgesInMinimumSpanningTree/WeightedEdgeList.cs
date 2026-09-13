namespace DSAExperimentation.LeetCode.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

// LC 1489's edge list in the form Kruskal wants it: the edges exactly as LeetCode
// gave them, so an edge keeps its original index identity (weights tie, so index is
// the only way to name "the edge being excluded" or "the edge being forced in"), plus
// the ascending-by-weight order the greedy scan walks.
//
// This exists as a type rather than a pair of loose arrays so a benchmark can hoist
// the sort into [GlobalSetup] and hand the prepared edge list to a strategy's second
// overload without that overload becoming ambiguous with the LeetCode-shaped one
// (ARCHITECTURE.md #17.4).
internal sealed class WeightedEdgeList
{
    // LeetCode states each edge as [from, to, weight].
    private const int WeightIndex = 2;

    private WeightedEdgeList(int nodeCount, int[][] edges, int[] byWeight)
    {
        NodeCount = nodeCount;
        Edges = edges;
        ByWeight = byWeight;
    }

    public int NodeCount { get; }

    // Indexed by LeetCode's own edge numbering, which is what the answer reports.
    public int[][] Edges { get; }

    // Edge indices in ascending weight order - Kruskal's scan order.
    public int[] ByWeight { get; }

    public static int WeightOf(int[] edge) => edge[WeightIndex];

    public static WeightedEdgeList Build(int nodeCount, int[][] edges)
    {
        var byWeight = Enumerable.Range(0, edges.Length).OrderBy(i => WeightOf(edges[i])).ToArray();

        return new WeightedEdgeList(nodeCount, edges, byWeight);
    }
}
