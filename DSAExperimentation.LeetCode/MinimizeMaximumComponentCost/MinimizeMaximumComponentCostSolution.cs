using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

// LeetCode 3613. Minimize Maximum Component Cost: remove edges so at most k
// connected components remain, minimizing the largest edge weight left in any
// component. Run Kruskal in reverse - start with every node isolated and merge
// components by ascending edge weight - and the weight of the (n-k)th
// successful merge is exactly the answer: every lighter edge either had to be
// added to get under k components, or was already inside one; no edge heavier
// than it ever needs to survive.
//
// ComponentNode/ComponentTopology/ComponentGraph (this folder) are a thin
// IEdgeTopology adapter for LeetCode's own (n, edges) shape, not a new
// algorithm - they exist only so the composed strategy below can hand
// MinimumSpanningTree.Kruskal a vertex list instead of re-deriving Kruskal's
// merge loop by hand.
internal static class MinimizeMaximumComponentCostSolution
{
    // The textbook Kruskal loop: BCL Array.Sort plus a hand-rolled union-find
    // array, the arm the composed strategy below has to justify itself against.
    public static int MinCostByUnionFind(int n, int[][] edges, int k)
    {
        if (k >= n)
        {
            return 0;
        }

        var sorted = (int[][])edges.Clone();
        Array.Sort(sorted, (left, right) => left[2].CompareTo(right[2]));

        var parent = new int[n];
        for (var i = 0; i < n; i++)
        {
            parent[i] = i;
        }

        return WeightAtComponentCount(parent, sorted, n, k);
    }

    // Kruskal's merge loop: edges are taken in ascending weight, and the first merge that
    // brings the component count down to k fixes the answer. Zero when the edges run out
    // first, which for k < n a connected input never lets happen.
    private static int WeightAtComponentCount(int[] parent, int[][] sorted, int n, int k)
    {
        var components = n;

        foreach (var edge in sorted)
        {
            (var done, components) = MergeEdge(parent, (edge[0], edge[1]), components, k);

            if (done)
            {
                return edge[2];
            }
        }

        return 0;
    }

    // One edge of the merge. Endpoints already sharing a component are skipped; otherwise the
    // two components join and the count comes back one smaller. `done` marks the merge that
    // brought the count down to k, whose edge weight is the answer.
    private static (bool Done, int Components) MergeEdge(
        int[] parent, (int NodeA, int NodeB) edge, int components, int k)
    {
        var (nodeA, nodeB) = edge;
        var rootA = Find(parent, nodeA);
        var rootB = Find(parent, nodeB);

        if (rootA == rootB)
        {
            return (false, components);
        }

        parent[rootA] = rootB;
        components--;

        return (components <= k, components);
    }

    private static int Find(int[] parent, int node)
    {
        while (parent[node] != node)
        {
            parent[node] = parent[parent[node]];
            node = parent[node];
        }

        return node;
    }

    // MinimumSpanningTree.Kruskal already IS "merge components by ascending
    // weight, keep only the edges that actually reduced the component count" -
    // its result list is that merge order, so the answer is just the weight at
    // index (n - k) - 1 (0 when k >= n, since no merge is needed at all). The
    // graph is guaranteed connected, so that index always exists once k < n.
    public static int MinCostByKruskalMst(int n, int[][] edges, int k)
    {
        var graph = ComponentGraph.Build(n, edges);
        return MinCostByKruskalMst(graph, k);
    }

    public static int MinCostByKruskalMst(ComponentGraph graph, int k)
    {
        var n = graph.Vertices.Count;

        if (k >= n)
        {
            return 0;
        }

        var spanningEdges = MinimumSpanningTree.Kruskal<
            ComponentNode, ComponentTopology, ListEdges<ComponentNode, int>, int>(graph.Vertices);

        return spanningEdges[n - k - 1].Weight;
    }
}
