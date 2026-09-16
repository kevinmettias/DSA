namespace DSAExperimentation.LeetCode.LongestPalindromicPathInGraph;

// n <= 14 lets every node's neighbor set live in one int bitmask -
// NeighborMask[i] has bit j set exactly when i and j are adjacent - so the
// bitmask-memo strategy below can compute "unused neighbors of u" as a single
// AND/NOT instead of scanning an adjacency list. Problem-local: nothing else in
// the catalogue needs a bitmask-adjacency graph yet.
internal sealed class LabeledGraph(int nodeCount, string label, int[] neighborMask)
{
    public int NodeCount { get; } = nodeCount;

    public string Label { get; } = label;

    public int[] NeighborMask { get; } = neighborMask;

    public static LabeledGraph Build(int nodeCount, int[][] edges, string label)
    {
        var neighborMask = new int[nodeCount];

        foreach (var edge in edges)
        {
            var (u, v) = (edge[0], edge[1]);
            neighborMask[u] |= 1 << v;
            neighborMask[v] |= 1 << u;
        }

        return new LabeledGraph(nodeCount, label, neighborMask);
    }
}
