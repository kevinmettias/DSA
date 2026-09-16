using DisjointSetOperations = DSAExperimentation.DataStructures.DisjointSet.DisjointSet;

namespace DSAExperimentation.LeetCode.MinimumCostWalkInWeightedGraph;

// LC 3108's whole graph, reduced to exactly what a query needs: which component
// each vertex falls in, and that component's running bitwise AND of every edge
// weight inside it. A walk may revisit edges and vertices freely, and revisiting
// every edge of a connected component at least once is always legal, so the
// minimum cost achievable between any two vertices in the same component is the
// AND of every edge weight in that component - AND only ever clears bits, so no
// walk can do better, and none of the component's edges can be skipped without
// possibly leaving a clearable bit set. This precomputes that once instead of
// re-deriving it per query.
//
// Composes DataStructures.DisjointSet.DisjointSet - the dense-int Operations layer,
// not KeyedDisjointSet<TKey>'s key-to-id wrapper - directly: LC's vertices already
// are the dense [0, vertexCount) ids DisjointSet expects, so there is no key to map, unlike
// MinimumSpanningTree's KeyedDisjointSet<TNode> use over arbitrary TNode vertices.
// Aliased to DisjointSetOperations for the same reason KeyedDisjointSet.cs needs
// it: DataStructures.DisjointSet is both this file's namespace segment and the
// type's own name.
internal sealed class WalkCostComponents(DisjointSetOperations components, int[] andByRoot)
{
    public static WalkCostComponents Build(int vertexCount, int[][] edges)
    {
        var components = new DisjointSetOperations(vertexCount);

        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        // -1 is all bits set, the identity element for AND - folding any weight
        // into it just returns that weight, the same "start from the identity"
        // shape a sum starts from 0.
        var andByRoot = new int[vertexCount];
        Array.Fill(andByRoot, -1);

        foreach (var edge in edges)
        {
            var root = components.Find(edge[0]);
            andByRoot[root] &= edge[2];
        }

        return new WalkCostComponents(components, andByRoot);
    }

    public int MinimumCost(int source, int target)
        => components.IsConnected(source, target)
            ? AndForSource(source)
            : LeetCodeAnswer.None;

    // The component-wide AND for whichever component source sits in - read only once
    // connectivity has already established that both vertices share one.
    private int AndForSource(int source) => andByRoot[components.Find(source)];
}
