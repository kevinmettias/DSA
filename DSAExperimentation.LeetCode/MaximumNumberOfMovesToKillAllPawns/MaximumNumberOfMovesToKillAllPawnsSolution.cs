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

        return Minimax((0, 0), positions.Length, distances, cache);
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
        var (distance, queue) = StartKnightSearch(startRow, startCol);
        ExpandKnightFrontier(distance, queue);

        return distance;
    }

    // The search state a knight BFS begins from: every square unvisited (-1) except
    // the origin, and a frontier holding only the origin.
    private static (int[,] Distance, Queue<(int Row, int Col)> Frontier) StartKnightSearch(int startRow, int startCol)
    {
        var distance = new int[BoardSize, BoardSize];
        var frontier = new Queue<(int Row, int Col)>();
        frontier.Enqueue((startRow, startCol));

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                distance[row, col] = -1;
            }
        }

        distance[startRow, startCol] = 0;
        return (distance, frontier);
    }

    // Drain the frontier, recording the move count into each square the first time
    // it is reached.
    private static void ExpandKnightFrontier(int[,] distance, Queue<(int Row, int Col)> frontier)
    {
        while (frontier.Count > 0)
        {
            var (row, col) = frontier.Dequeue();

            foreach (var (deltaRow, deltaCol) in KnightOffsets)
            {
                var nextRow = row + deltaRow;
                var nextCol = col + deltaCol;

                if (IsInside(nextRow, nextCol) && distance[nextRow, nextCol] == -1)
                {
                    distance[nextRow, nextCol] = distance[row, col] + 1;
                    frontier.Enqueue((nextRow, nextCol));
                }
            }
        }
    }

    // The board is a fixed BoardSize square, so both coordinates within it is one
    // idea the BFS should not restate.
    private static bool IsInside(int row, int col)
        => row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;

    // The recursion's state is exactly the (mask, last) pair its memo key is already
    // built from, so the two arrive as that one state rather than as two loose ints.
    private static int Minimax(
        (int Mask, int Last) state, int pawnCount, int[,] distances, Dictionary<(int, int), int> cache)
    {
        var fullMask = (1 << pawnCount) - 1;

        if (state.Mask == fullMask)
        {
            return 0;
        }

        if (cache.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var best = FoldPawnOutcomes(state, pawnCount, (distances, cache));

        cache[state] = best;
        return best;
    }

    // One ply of the brute-force minimax: the best grand total over every pawn
    // still uncaptured from `state`, each candidate backed by the recursion's own
    // total for the rest of the game.
    private static int FoldPawnOutcomes(
        (int Mask, int Last) state, int pawnCount, (int[,] Distances, Dictionary<(int, int), int> Cache) context)
    {
        var (mask, last) = state;
        var maximizing = BitOperations.PopCount((uint)mask) % 2 == 0;
        var best = maximizing ? int.MinValue : int.MaxValue;

        for (var pawn = 0; pawn < pawnCount; pawn++)
        {
            var bit = 1 << pawn;

            if ((mask & bit) != 0)
            {
                continue;
            }

            var total = context.Distances[last, pawn + 1]
                + Minimax((mask | bit, pawn + 1), pawnCount, context.Distances, context.Cache);
            best = maximizing ? Math.Max(best, total) : Math.Min(best, total);
        }

        return best;
    }

    // This repo's own composition: knight-move distances via Reduce.Graph over
    // the KnightTopology witness (this folder), the minimax recursion via
    // Memoizer.
    public static int MaxMovesByReduceGraphMinimax(int kx, int ky, int[][] positions)
    {
        var knightDistances = BuildKnightDistances(kx, ky, positions);

        return MaxMovesByReduceGraphMinimax(knightDistances);
    }

    public static int MaxMovesByReduceGraphMinimax(KnightDistances knightDistances)
    {
        var (distances, pawnCount) = knightDistances;

        return Memoizer.Memoize<(int Mask, int Last), int>(
            (0, 0), new MovesStillToCome(distances, pawnCount));
    }

    // The composed arm's ply: the same fold as the brute-force arm, except each
    // candidate is backed by the memoizer's own handle on the rule for the rest of
    // the game.
    private static int FoldMemoizedPawnOutcomes(
        (int Mask, int Last) state,
        int pawnCount,
        (int[,] Distances, IRecurrence<(int Mask, int Last), int> Recurrence) context)
    {
        var (mask, last) = state;
        var maximizing = BitOperations.PopCount((uint)mask) % 2 == 0;
        var outcome = maximizing ? int.MinValue : int.MaxValue;

        for (var pawn = 0; pawn < pawnCount; pawn++)
        {
            var bit = 1 << pawn;

            if ((mask & bit) != 0)
            {
                continue;
            }

            var total = context.Distances[last, pawn + 1]
                + context.Recurrence.Replay((mask | bit, pawn + 1), context.Recurrence);
            outcome = maximizing ? Math.Max(outcome, total) : Math.Min(outcome, total);
        }

        return outcome;
    }

    // The prepared input the composed strategy's hoisted overload takes - built
    // from LeetCode's own shape here, or directly by a benchmark's
    // [GlobalSetup] so every BFS is charged to setup rather than the measured
    // minimax call.
    public static KnightDistances BuildKnightDistances(int kx, int ky, int[][] positions)
    {
        var points = BuildPoints(kx, ky, positions);
        var grid = new Grid(AllPassable());
        var distances = DistanceMatrixByReduceGraph(points, grid);

        return new KnightDistances(distances, positions.Length);
    }

    // One Reduce.Graph reachability search per point of interest, read off into the
    // pairwise distance matrix.
    private static int[,] DistanceMatrixByReduceGraph((int Row, int Col)[] points, Grid grid)
    {
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

        return distances;
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

    // The minimax rule, named: the optimal total moves still to come from a (mask, last)
    // state, where whoever's turn popcount(mask) makes it decides whether the remaining
    // pawns maximize or minimize. The pairwise distances and the pawn count are fixed for
    // the whole game and arrive once through the primary constructor; `rest` is the memo
    // run's own handle on this rule.
    private sealed class MovesStillToCome(int[,] distances, int pawnCount)
        : IRecurrence<(int Mask, int Last), int>
    {
        /// <inheritdoc/>
        public int Replay((int Mask, int Last) state, IRecurrence<(int Mask, int Last), int> rest)
        {
            if (state.Mask == (1 << pawnCount) - 1)
            {
                return 0;
            }

            return FoldMemoizedPawnOutcomes(state, pawnCount, (distances, rest));
        }
    }
}
