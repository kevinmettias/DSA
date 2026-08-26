using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.KeyedDisjointSet;

namespace DSAExperimentation.Algorithms.MinimumSpanningTrees;

// Folder/namespace is plural (MinimumSpanningTrees) while the class stays singular
// (MinimumSpanningTree) - the same split ShortestPaths/ShortestPath already uses,
// and for the same reason: a bare same-named reference from a namespace segment
// with the identical name has no type-argument list to disambiguate it (§10.3),
// unlike KeyedDisjointSet<TKey>'s own arity-based escape.
//
// Reuses ShortestPath's own IEdgeTopology/IEdges contracts rather than inventing a
// new one - both are Graph-domain algorithms over the same "weighted edge out of a
// node" shape, so this is the same-domain reuse ARCHITECTURE.md §5 step 5 endorses,
// not the cross-domain borrowing it forbids.
//
// TWeight is only IComparable<TWeight>, not INumber<TWeight> like ShortestPath -
// Kruskal sorts edges by weight but never adds them, so this is the weaker,
// sufficient constraint (the same reasoning behind BinarySearch's default
// IComparable<T> overload). Any INumber type already satisfies IComparable, so
// ShortestPath's own weighted-graph test fixtures work here unmodified.
//
// IEdgeTopology is per-node/directed - built for Dijkstra/A*'s single-direction
// relaxation - so a caller storing an undirected graph as symmetric adjacency will
// have each edge discovered twice, once from each endpoint. That only doubles the
// candidate list; it never changes which forest comes out, since the second
// discovery is always rejected by the IsConnected check below - the same
// "changes cost, not output" distinction DisjointSet's own linking policy relies on.
//
// A disconnected input graph is not an error: the scan below naturally produces a
// minimum spanning forest (one tree per connected component) rather than a single
// tree, which is Kruskal's well-known behavior, not a case this needs to guard
// against.
//
// Composes DataStructures.DisjointSet.KeyedDisjointSet<TKey>, which wraps this
// repo's dense-int-indexed DisjointSet with a key-assignment layer (its own doc
// comment explains the naming/placement). TryUnion's bool result covers a
// discovered edge whose neighbor falls outside the given vertices set (an
// incomplete-vertices precondition violation on the caller's part) - it's simply
// excluded rather than thrown on, the same "unchecked precondition" treatment
// BinarySearch's sortedness gets.
internal static class MinimumSpanningTree
{
    public static List<(TNode A, TNode B, TWeight Weight)> Kruskal<TNode, TTopology, TEdges, TWeight>(
        IEnumerable<TNode> vertices)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : IComparable<TWeight>
    {
        var vertexList = vertices.ToList();
        var candidates = CollectEdges<TNode, TTopology, TEdges, TWeight>(vertexList);
        candidates.Sort((left, right) => left.Weight.CompareTo(right.Weight));

        var components = new KeyedDisjointSet<TNode>(vertexList);
        var spanningEdges = new List<(TNode A, TNode B, TWeight Weight)>();

        foreach (var edge in candidates)
        {
            if (components.IsConnected(edge.A, edge.B))
            {
                continue;
            }

            if (components.TryUnion(edge.A, edge.B))
            {
                spanningEdges.Add(edge);
            }
        }

        return spanningEdges;
    }

    private static List<(TNode A, TNode B, TWeight Weight)> CollectEdges<TNode, TTopology, TEdges, TWeight>(
        List<TNode> vertices)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
    {
        var edges = new List<(TNode A, TNode B, TWeight Weight)>();

        foreach (var vertex in vertices)
        {
            var neighbors = TTopology.GetEdges(vertex);

            for (var i = 0; i < neighbors.Count; i++)
            {
                var (weight, neighbor) = neighbors.Get(i);
                edges.Add((vertex, neighbor, weight));
            }
        }

        return edges;
    }
}
