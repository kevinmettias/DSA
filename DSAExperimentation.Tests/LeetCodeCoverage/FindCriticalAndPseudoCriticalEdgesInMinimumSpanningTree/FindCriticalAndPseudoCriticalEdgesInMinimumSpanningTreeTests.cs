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
        var mst = new MstInput(n, edges, SortEdgesByWeight(edges));

        var baselineWeight = BaselineWeight(mst);

        return ClassifyEdges(mst, baselineWeight);
    }

    private static int[] SortEdgesByWeight(int[][] edges)
        => Enumerable.Range(0, edges.Length).OrderBy(i => edges[i][2]).ToArray();

    private static int BaselineWeight(MstInput mst)
        => MstWeight(mst, new EdgeOverride(SkipIndex: -1, ForceIndex: -1))
            ?? throw new InvalidOperationException("LeetCode 1489 guarantees a connected input graph.");

    private static (List<int> Critical, List<int> PseudoCritical) ClassifyEdges(MstInput mst, int baselineWeight)
    {
        var critical = new List<int>();
        var pseudoCritical = new List<int>();

        for (var i = 0; i < mst.Edges.Length; i++)
        {
            var withoutEdge = MstWeight(mst, new EdgeOverride(SkipIndex: i, ForceIndex: -1));

            if (withoutEdge is null || withoutEdge > baselineWeight)
            {
                critical.Add(i);
                continue;
            }

            if (MstWeight(mst, new EdgeOverride(SkipIndex: -1, ForceIndex: i)) == baselineWeight)
            {
                pseudoCritical.Add(i);
            }
        }

        return (critical, pseudoCritical);
    }

    private static int? MstWeight(MstInput mst, EdgeOverride overrideEdge)
    {
        var components = new DisjointSet(mst.N);
        var totalWeight = 0;
        var edgesUsed = 0;

        if (overrideEdge.ForceIndex >= 0)
        {
            var forcedWeight = ForceEdge(mst, overrideEdge.ForceIndex, components);
            Accumulate(forcedWeight, ref totalWeight, ref edgesUsed);
        }

        foreach (var index in mst.EdgesByWeight)
        {
            var addedWeight = TryUnionEdge(mst, overrideEdge, components, index);
            Accumulate(addedWeight, ref totalWeight, ref edgesUsed);
        }

        return edgesUsed == mst.N - 1 ? totalWeight : null;
    }

    private static void Accumulate(int? addedWeight, ref int totalWeight, ref int edgesUsed)
    {
        if (addedWeight is null)
        {
            return;
        }

        totalWeight += addedWeight.Value;
        edgesUsed++;
    }

    private static int ForceEdge(MstInput mst, int forceIndex, DisjointSet components)
    {
        var forced = mst.Edges[forceIndex];
        components.Union(forced[0], forced[1]);
        return forced[2];
    }

    private static int? TryUnionEdge(MstInput mst, EdgeOverride overrideEdge, DisjointSet components, int index)
    {
        if (index == overrideEdge.SkipIndex || index == overrideEdge.ForceIndex)
        {
            return null;
        }

        var edge = mst.Edges[index];

        if (components.IsConnected(edge[0], edge[1]))
        {
            return null;
        }

        components.Union(edge[0], edge[1]);
        return edge[2];
    }

    private readonly record struct MstInput(int N, int[][] Edges, int[] EdgesByWeight);

    private readonly record struct EdgeOverride(int SkipIndex, int ForceIndex);
}
