using System.Diagnostics.CodeAnalysis;
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
// This file is the walk itself: the entry point and the relaxation that refines the matrix in
// place. Producing the matrix it refines - the index map, the default cells, the seeded direct
// edges, all of it built from the same IEdgeTopology/IEdges contracts - is a separate
// responsibility with no state in common with the refinement, and lives in
// DenseDistanceMatrix.cs.
internal static class AllPairsShortestPaths
{
    public static bool TryComputeDistances<TNode, TTopology, TEdges, TWeight>(
        IEnumerable<TNode> vertices, out Dictionary<(TNode From, TNode To), TWeight> distances)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        var vertexList = vertices.ToList();
        var index = DenseDistanceMatrix.BuildIndex(vertexList);
        var matrix = DenseDistanceMatrix.BuildInitialMatrix<TNode, TTopology, TEdges, TWeight>(
            vertexList, index);

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

    private static void Refine<TWeight>(TWeight[,] matrix, int count)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        for (var k = 0; k < count; k++)
        {
            for (var i = 0; i < count; i++)
            {
                RelaxRow(matrix, i, k, count);
            }
        }
    }

    private static void RelaxRow<TWeight>(TWeight[,] matrix, int sourceIndex, int intermediateIndex, int count)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        // Guards against TWeight.MaxValue + TWeight.MaxValue, which would otherwise
        // silently overflow under generic math and corrupt the matrix with a
        // wrapped-around value that reads as a real (very wrong) distance instead of
        // "still unreached." Forced purely by the dense matrix needing every cell to
        // hold some value - not a caller-facing precondition the way non-negative
        // weights is for ShortestPath.cs.
        if (matrix[sourceIndex, intermediateIndex] == TWeight.MaxValue)
        {
            return;
        }

        for (var j = 0; j < count; j++)
        {
            RelaxCell(matrix, sourceIndex, intermediateIndex, j);
        }
    }

    private static void RelaxCell<TWeight>(TWeight[,] matrix, int sourceIndex, int intermediateIndex,
                                          int destinationIndex)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        if (matrix[intermediateIndex, destinationIndex] == TWeight.MaxValue)
        {
            return;
        }

        if (!TryAddChecked(matrix[sourceIndex, intermediateIndex],
                           matrix[intermediateIndex, destinationIndex], out var candidate))
        {
            return;
        }

        UpdateIfShorter(matrix, sourceIndex, destinationIndex, candidate);
    }

    // [MaybeNullWhen(false)] is what lets the overflow branch assign `default` rather than
    // `default!`: it declares in the signature that `sum` is only read when this returns
    // true, which is exactly the contract the assignment used to assert.
    private static bool TryAddChecked<TWeight>(TWeight first, TWeight second,
                                              [MaybeNullWhen(false)] out TWeight sum)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        // Two finite (non-sentinel) weights can still sum past TWeight's own
        // representable range - e.g. two large-magnitude negative edges, which
        // negative-cycle detection makes a legitimate input here - and silently
        // wrap into a value that reads as a real (very wrong) distance instead of
        // "still unreached." checked() only has an effect for TWeight instances
        // that supply a checked addition operator (int/long do); for others this
        // behaves exactly as before.
        try
        {
            sum = checked(first + second);

            return true;
        }
        catch (OverflowException)
        {
            sum = default;

            return false;
        }
    }

    private static void UpdateIfShorter<TWeight>(TWeight[,] matrix, int sourceIndex, int destinationIndex,
                                                 TWeight candidate)
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
    {
        if (candidate < matrix[sourceIndex, destinationIndex])
        {
            matrix[sourceIndex, destinationIndex] = candidate;
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
