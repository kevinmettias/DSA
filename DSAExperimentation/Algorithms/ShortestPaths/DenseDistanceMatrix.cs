using System.Numerics;

using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// Builds the starting matrix of the Floyd-Warshall walk in AllPairsShortestPaths.cs: a dense
// V-by-V grid carrying zero on the diagonal and the "still unreached" sentinel in every other
// cell, seeded from the graph's own direct edges before the first candidate intermediate vertex
// is tried. Refining that matrix is the walk's own responsibility and stays with it; this type
// holds no state and no refinement step, only the construction of what the walk starts from.
//
// Reuses ShortestPath.cs's own IEdgeTopology/IEdges purely to seed the initial matrix - the
// same same-domain reuse MinimumSpanningTree.cs already established for the identical
// contracts. The matrix itself is Operations-internal scratch state, the same bucket
// ShortestPath's own Heap frontier is in, not a second Representation competing for its own
// interface.
//
// The index map is local scratch state - the same bucket ShortestPath.Distances and
// TopologicalSort.inDegree already occupy - not a reusable DataStructures-tier wrapper, so it's
// a plain Dictionary<TNode,int>, not this repo's own HashMap the way KeyedDisjointSet<TKey>
// composes one for its comparable-looking dense-core-plus-id-map shape.
internal static class DenseDistanceMatrix
{
    private const string ReservedEdgeWeightMessage = "An edge weight equal to TWeight.MaxValue is reserved to mean \"still unreached\" and cannot be a real edge weight.";

    internal static Dictionary<TNode, int> BuildIndex<TNode>(List<TNode> vertices)
        where TNode : class
    {
        var index = new Dictionary<TNode, int>();

        for (var i = 0; i < vertices.Count; i++)
        {
            index[vertices[i]] = i;
        }

        return index;
    }

    internal static TWeight[,] BuildInitialMatrix<TNode, TTopology, TEdges, TWeight>(
        List<TNode> vertices, Dictionary<TNode, int> index)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        var count = vertices.Count;
        var matrix = new TWeight[count, count];

        for (var i = 0; i < count; i++)
        {
            for (var j = 0; j < count; j++)
            {
                matrix[i, j] = i == j ? TWeight.Zero : TWeight.MaxValue;
            }
        }

        foreach (var vertex in vertices)
        {
            SeedRow<TNode, TTopology, TEdges, TWeight>(vertex, matrix, index);
        }

        return matrix;
    }

    private static void SeedRow<TNode, TTopology, TEdges, TWeight>(
        TNode vertex, TWeight[,] matrix, Dictionary<TNode, int> index)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        var from = index[vertex];
        var neighbors = TTopology.GetEdges(vertex);

        for (var i = 0; i < neighbors.Count; i++)
        {
            var (weight, neighbor) = neighbors.Get(i);

            RelaxEdge(matrix, index, new PendingEdge<TNode, TWeight>(from, weight, neighbor));
        }
    }

    private static void RelaxEdge<TNode, TWeight>(
        TWeight[,] matrix, Dictionary<TNode, int> index, PendingEdge<TNode, TWeight> edge)
        where TNode : class
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        // Unlike the caller-omitted-vertex case just below, a weight colliding with the
        // "still unreached" sentinel isn't a wrong-but-plausible answer we can silently
        // let ride - it would make a real edge indistinguishable from no edge at all in
        // every later read of this cell, so this one precondition is validated rather
        // than documented-and-trusted.
        if (edge.Weight == TWeight.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(edge), ReservedEdgeWeightMessage);
        }

        // An edge whose target lies outside `vertices` is silently skipped, not an
        // exception - the same "wrong/incomplete answer, never a throw" convention
        // TopologicalSort's GetValueOrDefault-guarded child lookup already uses for an
        // analogous caller-omitted-vertex precondition.
        if (!index.TryGetValue(edge.Neighbor, out var to))
        {
            return;
        }

        if (edge.Weight < matrix[edge.From, to])
        {
            matrix[edge.From, to] = edge.Weight;
        }
    }

    // One direct edge as the seeding pass hands it to the cell it fills: the row it starts
    // from, its weight, and the vertex it reaches. Nested rather than namespace-level
    // because this is the only type that constructs or reads one.
    private readonly record struct PendingEdge<TNode, TWeight>(int From, TWeight Weight, TNode Neighbor)
        where TNode : class
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>;
}
