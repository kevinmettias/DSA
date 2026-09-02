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

        var components = n;

        foreach (var edge in sorted)
        {
            var rootA = Find(parent, edge[0]);
            var rootB = Find(parent, edge[1]);

            if (rootA == rootB)
            {
                continue;
            }

            parent[rootA] = rootB;
            components--;

            if (components <= k)
            {
                return edge[2];
            }
        }

        return 0;
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
    public static int MinCostByKruskalMst(int n, int[][] edges, int k) =>
        MinCostByKruskalMst(ComponentGraph.Build(n, edges), k);

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
