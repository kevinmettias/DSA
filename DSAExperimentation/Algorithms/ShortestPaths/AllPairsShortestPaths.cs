using System.Numerics;

using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// Floyd-Warshall: every pair's distance at once, via a dense V-by-V matrix refined over every
// candidate intermediate vertex - a genuinely different shape from ShortestPath.cs's
// single-source frontier walk, not a tiered refinement of it, so it gets its own file/class
// rather than a fourth method there. Named for what it computes (all-pairs distances), not who
// invented it - unlike BellmanFord.cs, a real descriptive name is available here that doesn't
// collide with anything else in this folder.
//
// Reuses ShortestPath.cs's own IEdgeTopology/IEdges purely to seed the initial matrix - the
// same same-domain reuse MinimumSpanningTree.cs already established for the identical
// contracts. The matrix itself is Operations-internal scratch state, the same bucket
// ShortestPath's own Heap frontier is in, not a second Representation competing for its own
// interface.
//
// The index map below is local scratch state - the same bucket ShortestPath.Distances and
// TopologicalSort.inDegree already occupy - not a reusable DataStructures-tier wrapper, so it's
// a plain Dictionary<TNode,int>, not this repo's own HashMap the way KeyedDisjointSet<TKey>
// composes one for its comparable-looking dense-core-plus-id-map shape.
internal static class AllPairsShortestPaths
{
    private const string ReservedEdgeWeightMessage = "An edge weight equal to TWeight.MaxValue is reserved to mean \"still unreached\" and cannot be a real edge weight.";

    public static bool TryComputeDistances<TNode, TTopology, TEdges, TWeight>(
        IEnumerable<TNode> vertices, out Dictionary<(TNode From, TNode To), TWeight> distances)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        var vertexList = vertices.ToList();
        var index = BuildIndex(vertexList);
        var matrix = BuildInitialMatrix<TNode, TTopology, TEdges, TWeight>(vertexList, index);

        Refine(matrix, vertexList.Count);

        distances = ToDistances(vertexList, matrix);

        for (var i = 0; i < vertexList.Count; i++)
        {
            if (matrix[i, i] < TWeight.Zero)
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<TNode, int> BuildIndex<TNode>(List<TNode> vertices)
        where TNode : class
    {
        var index = new Dictionary<TNode, int>();

        for (var i = 0; i < vertices.Count; i++)
        {
            index[vertices[i]] = i;
        }

        return index;
    }

    private static TWeight[,] BuildInitialMatrix<TNode, TTopology, TEdges, TWeight>(
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
            var from = index[vertex];
            var neighbors = TTopology.GetEdges(vertex);

            for (var i = 0; i < neighbors.Count; i++)
            {
                var (weight, neighbor) = neighbors.Get(i);

                // Unlike the caller-omitted-vertex case just below, a weight colliding with the
                // "still unreached" sentinel isn't a wrong-but-plausible answer we can silently
                // let ride - it would make a real edge indistinguishable from no edge at all in
                // every later read of this cell, so this one precondition is validated rather
                // than documented-and-trusted.
                if (weight == TWeight.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(vertices), ReservedEdgeWeightMessage);
                }

                // An edge whose target lies outside `vertices` is silently skipped, not an
                // exception - the same "wrong/incomplete answer, never a throw" convention
                // TopologicalSort's GetValueOrDefault-guarded child lookup already uses for an
                // analogous caller-omitted-vertex precondition.
                if (!index.TryGetValue(neighbor, out var to))
                {
                    continue;
                }

                if (weight < matrix[from, to])
                {
                    matrix[from, to] = weight;
                }
            }
        }

        return matrix;
    }

    private static void Refine<TWeight>(TWeight[,] matrix, int count)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        for (var k = 0; k < count; k++)
        {
            for (var i = 0; i < count; i++)
            {
                // Guards against TWeight.MaxValue + TWeight.MaxValue, which would otherwise
                // silently overflow under generic math and corrupt the matrix with a
                // wrapped-around value that reads as a real (very wrong) distance instead of
                // "still unreached." Forced purely by the dense matrix needing every cell to
                // hold some value - not a caller-facing precondition the way non-negative
                // weights is for ShortestPath.cs.
                if (matrix[i, k] == TWeight.MaxValue)
                {
                    continue;
                }

                for (var j = 0; j < count; j++)
                {
                    if (matrix[k, j] == TWeight.MaxValue)
                    {
                        continue;
                    }

                    // Two finite (non-sentinel) weights can still sum past TWeight's own
                    // representable range - e.g. two large-magnitude negative edges, which
                    // negative-cycle detection makes a legitimate input here - and silently
                    // wrap into a value that reads as a real (very wrong) distance instead of
                    // "still unreached." checked() only has an effect for TWeight instances
                    // that supply a checked addition operator (int/long do); for others this
                    // behaves exactly as before.
                    TWeight candidate;

                    try
                    {
                        candidate = checked(matrix[i, k] + matrix[k, j]);
                    }
                    catch (OverflowException)
                    {
                        continue;
                    }

                    if (candidate < matrix[i, j])
                    {
                        matrix[i, j] = candidate;
                    }
                }
            }
        }
    }

    private static Dictionary<(TNode From, TNode To), TWeight> ToDistances<TNode, TWeight>(
        List<TNode> vertices, TWeight[,] matrix)
        where TNode : class
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        var distances = new Dictionary<(TNode From, TNode To), TWeight>();

        for (var i = 0; i < vertices.Count; i++)
        {
            for (var j = 0; j < vertices.Count; j++)
            {
                // (v, v) is kept even when it's the negative-cycle signal itself (a negative
                // diagonal), not just on success - only ever omitted (like any other pair) once
                // it's the TWeight.MaxValue "still unreached" sentinel, which a self-pair never is.
                if (matrix[i, j] == TWeight.MaxValue)
                {
                    continue;
                }

                distances[(vertices[i], vertices[j])] = matrix[i, j];
            }
        }

        return distances;
    }
}
