using System.Numerics;

using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

// LeetCode 3283. Maximum Number of Moves to Kill All Pawns: Alice and Bob
// alternate turns (Alice first) moving one shared knight to capture a remaining
// pawn in the fewest possible knight moves. Alice maximizes the grand total move
// count over the whole game, Bob minimizes it. With positions.length <= 15, this
// is bitmask-TSP with an alternating min/max objective instead of a single sum:
// dp(mask, last) is the optimal total moves still to come, having already
// captured `mask`'s pawns and standing on `last`'s square (index 0 = the
// knight's start, 1..pawnCount = the pawns). Whoever's turn it is - Alice on an
// even popcount(mask), Bob on odd - decides whether dp maximizes or minimizes
// over the remaining pawns.
//
// Both strategies need the same input: pairwise knight-move distances between
// the start and every pawn. They differ in where those distances and the
// minimax come from - a hand-rolled BFS plus a hand-rolled Dictionary-memoized
// recursion, or this repo's own Reduce.Graph (over a knight-move IGraphTopology
// local to this problem, since GridChildren's 4-orthogonal offsets don't fit a
// knight) plus Memoizer.
internal static class MaximumNumberOfMovesToKillAllPawnsSolution
{
    private const int BoardSize = 50;

    private static readonly (int DeltaRow, int DeltaCol)[] KnightOffsets =
    [
        (-2, -1), (-2, 1), (-1, -2), (-1, 2),
        (1, -2), (1, 2), (2, -1), (2, 1),
    ];

    // Textbook baseline: a BCL Queue-based BFS per point of interest for the
    // distance matrix (no repo topology/traversal primitives), and a hand-rolled
    // Dictionary-memoized minimax recursion (no repo Memoizer) for the game
    // itself. The arm the composed strategy below has to justify itself against.
    public static int MaxMovesByBruteForceMinimax(int kx, int ky, int[][] positions)
    {
        var points = BuildPoints(kx, ky, positions);
        var distances = BruteForceDistanceMatrix(points);
        var cache = new Dictionary<(int Mask, int Last), int>();

        return Minimax(mask: 0, last: 0, positions.Length, distances, cache);
    }

    private static int[,] BruteForceDistanceMatrix((int Row, int Col)[] points)
    {
        var distances = new int[points.Length, points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            var reach = BreadthFirstKnightDistances(points[i].Row, points[i].Col);

            for (var j = 0; j < points.Length; j++)
            {
                distances[i, j] = reach[points[j].Row, points[j].Col];
            }
        }

        return distances;
    }

    private static int[,] BreadthFirstKnightDistances(int startRow, int startCol)
    {
        var distance = new int[BoardSize, BoardSize];

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                distance[row, col] = -1;
            }
        }

        distance[startRow, startCol] = 0;
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((startRow, startCol));

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            foreach (var (deltaRow, deltaCol) in KnightOffsets)
            {
                var nextRow = row + deltaRow;
                var nextCol = col + deltaCol;

                if (nextRow >= 0 && nextRow < BoardSize && nextCol >= 0 && nextCol < BoardSize &&
                    distance[nextRow, nextCol] == -1)
                {
                    distance[nextRow, nextCol] = distance[row, col] + 1;
                    queue.Enqueue((nextRow, nextCol));
                }
            }
        }

        return distance;
    }

    private static int Minimax(int mask, int last, int pawnCount, int[,] distances, Dictionary<(int, int), int> cache)
    {
        var fullMask = (1 << pawnCount) - 1;

        if (mask == fullMask)
        {
            return 0;
        }

        if (cache.TryGetValue((mask, last), out var cached))
        {
            return cached;
        }

        var maximizing = BitOperations.PopCount((uint)mask) % 2 == 0;
        var best = maximizing ? int.MinValue : int.MaxValue;

        for (var pawn = 0; pawn < pawnCount; pawn++)
        {
            var bit = 1 << pawn;

            if ((mask & bit) != 0)
            {
                continue;
            }

            var total = distances[last, pawn + 1] + Minimax(mask | bit, pawn + 1, pawnCount, distances, cache);
            best = maximizing ? Math.Max(best, total) : Math.Min(best, total);
        }

        cache[(mask, last)] = best;
        return best;
    }

    // This repo's own composition: knight-move distances via Reduce.Graph over
    // the KnightTopology witness (this folder), the minimax recursion via
    // Memoizer.
    public static int MaxMovesByReduceGraphMinimax(int kx, int ky, int[][] positions) =>
        MaxMovesByReduceGraphMinimax(BuildKnightDistances(kx, ky, positions));

    public static int MaxMovesByReduceGraphMinimax(KnightDistances knightDistances)
    {
        var (distances, pawnCount) = knightDistances;
        var fullMask = (1 << pawnCount) - 1;

        int Recurrence((int Mask, int Last) state, Func<(int, int), int> best)
        {
            var (mask, last) = state;

            if (mask == fullMask)
            {
                return 0;
            }

            var maximizing = BitOperations.PopCount((uint)mask) % 2 == 0;
            var outcome = maximizing ? int.MinValue : int.MaxValue;

            for (var pawn = 0; pawn < pawnCount; pawn++)
            {
                var bit = 1 << pawn;

                if ((mask & bit) != 0)
                {
                    continue;
                }

                var total = distances[last, pawn + 1] + best((mask | bit, pawn + 1));
                outcome = maximizing ? Math.Max(outcome, total) : Math.Min(outcome, total);
            }

            return outcome;
        }

        return Memoizer.Memoize<(int Mask, int Last), int>((0, 0), Recurrence);
    }

    // The prepared input the composed strategy's hoisted overload takes - built
    // from LeetCode's own shape here, or directly by a benchmark's
    // [GlobalSetup] so every BFS is charged to setup rather than the measured
    // minimax call.
    public static KnightDistances BuildKnightDistances(int kx, int ky, int[][] positions)
    {
        var points = BuildPoints(kx, ky, positions);
        var grid = new Grid(AllPassable());
        var distances = new int[points.Length, points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            var source = new GridNode(points[i].Row, points[i].Col, grid);
            var reach = Reduce.Graph<
                GridNode, KnightTopology, KnightChildren,
                NaturalChildOrder<GridNode, KnightChildren>, KnightChildren,
                BreadthFirstReduceOrder<GridNode>,
                DistanceMapReduceAlgebra<GridNode>, Dictionary<GridNode, int>>(source);

            for (var j = 0; j < points.Length; j++)
            {
                var target = new GridNode(points[j].Row, points[j].Col, grid);
                distances[i, j] = reach.TryGetValue(target, out var distance) ? distance : -1;
            }
        }

        return new KnightDistances(distances, positions.Length);
    }

    private static bool[,] AllPassable()
    {
        var passable = new bool[BoardSize, BoardSize];

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                passable[row, col] = true;
            }
        }

        return passable;
    }

    // Index 0 = the knight's start; 1..positions.Length = the pawns, in their
    // original order - the shared indexing both distance-matrix builders and
    // both minimax recursions agree on.
    private static (int Row, int Col)[] BuildPoints(int kx, int ky, int[][] positions)
    {
        var points = new (int Row, int Col)[positions.Length + 1];
        points[0] = (kx, ky);

        for (var i = 0; i < positions.Length; i++)
        {
            points[i + 1] = (positions[i][0], positions[i][1]);
        }

        return points;
    }
}
