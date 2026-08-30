using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

// LeetCode 1489. Find Critical and Pseudo-Critical Edges in Minimum Spanning Tree:
// runs Kruskal's algorithm - via this repo's own DisjointSet union-find, the same
// primitive Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal composes
// internally - three ways per edge: once to get the baseline MST weight, once with
// that edge excluded (critical if the MST weight rises or the graph disconnects),
// and once with that edge forced in first (pseudo-critical if the resulting weight
// still matches the baseline). MinimumSpanningTree.Kruskal itself isn't reused
// directly because its IEdgeTopology-based edge discovery has no notion of "skip
// edge #i" or "force edge #i in" - both need per-edge index identity (weights can
// tie), which a direct DisjointSet-based Kruskal loop gives for free.
public sealed partial class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeTests
{
    [Fact]
    public void Classify_LeetCodeExampleWithTies_SplitsCriticalFromPseudoCritical()
    {
        int[][] edges =
        [
            [0, 1, 1], [1, 2, 1], [2, 3, 2], [0, 3, 2], [0, 4, 3], [3, 4, 3], [1, 4, 6],
        ];

        var (critical, pseudoCritical) = Classify(5, edges);

        Assert.Equal([0, 1], critical);
        Assert.Equal([2, 3, 4, 5], pseudoCritical);
    }

    [Fact]
    public void Classify_FourWayTieAroundASquare_EveryEdgeIsPseudoCriticalNoneCritical()
    {
        int[][] edges = [[0, 1, 1], [1, 2, 1], [2, 3, 1], [0, 3, 1]];

        var (critical, pseudoCritical) = Classify(4, edges);

        Assert.Empty(critical);
        Assert.Equal([0, 1, 2, 3], pseudoCritical);
    }

    private static (List<int> Critical, List<int> PseudoCritical) Classify(int n, int[][] edges)
    {
        var edgesByWeight = Enumerable.Range(0, edges.Length).OrderBy(i => edges[i][2]).ToArray();
        var baselineWeight = MstWeight(n, edges, edgesByWeight, skipIndex: -1, forceIndex: -1)
            ?? throw new InvalidOperationException("LeetCode 1489 guarantees a connected input graph.");

        var critical = new List<int>();
        var pseudoCritical = new List<int>();

        for (var i = 0; i < edges.Length; i++)
        {
            var withoutEdge = MstWeight(n, edges, edgesByWeight, skipIndex: i, forceIndex: -1);

            if (withoutEdge is null || withoutEdge > baselineWeight)
            {
                critical.Add(i);
                continue;
            }

            if (MstWeight(n, edges, edgesByWeight, skipIndex: -1, forceIndex: i) == baselineWeight)
            {
                pseudoCritical.Add(i);
            }
        }

        return (critical, pseudoCritical);
    }

    private static int? MstWeight(int n, int[][] edges, int[] edgesByWeight, int skipIndex, int forceIndex)
    {
        var components = new DisjointSet(n);
        var totalWeight = 0;
        var edgesUsed = 0;

        if (forceIndex >= 0)
        {
            var forced = edges[forceIndex];
            components.Union(forced[0], forced[1]);
            totalWeight += forced[2];
            edgesUsed++;
        }

        foreach (var index in edgesByWeight)
        {
            if (index == skipIndex || index == forceIndex)
            {
                continue;
            }

            var edge = edges[index];

            if (components.IsConnected(edge[0], edge[1]))
            {
                continue;
            }

            components.Union(edge[0], edge[1]);
            totalWeight += edge[2];
            edgesUsed++;
        }

        return edgesUsed == n - 1 ? totalWeight : null;
    }
}
