using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

// LeetCode 4003. Minimum Cost Path with Alternating Directions III: reach
// (m - 1, n - 1) from (0, 0), where every action after entering (0, 0) must
// alternate between "right or down" (odd-numbered actions) and "left or up"
// (even-numbered), waiting in place always being allowed instead. Both moving
// against the current parity and waiting cost the source cell's own penalty on
// top of the destination's entrance cost ((row + 1) * (col + 1)); a move that
// respects parity costs only the entrance.
//
// (0, 0)'s own entrance cost is paid once, unconditionally, before the first
// action - it does not depend on which parity state search eventually reaches
// (m - 1, n - 1) in, so both strategies add it once at the end (it is always 1)
// rather than seeding it into the search itself. That leaves a plain non-negative
// -weight shortest path over the state space (row, col, next-action-parity):
// up to 2 * m * n states, up to five out-edges each (four moves plus wait).
//
// The two strategies are the same search; they differ only in what runs it - a
// hand-rolled BCL Dijkstra (PriorityQueue<T, TPriority> plus Dictionary, the
// "what you'd write without this repo" arm) against ShortestPath.Dijkstra
// composed with AlternatingGridTopology's on-the-fly edges.
internal static class MinimumCostPathWithAlternatingDirectionsIIISolution
{
    private const long StartEntranceCost = 1; // (0 + 1) * (0 + 1), always.

    public static long MinCostByBclDijkstra(int rowCount, int columnCount, int[][] penalty)
    {
        var start = (Row: 0, Col: 0, NextActionIsOdd: true);
        var distances = new Dictionary<(int Row, int Col, bool NextActionIsOdd), long> { [start] = 0 };
        var frontier = new PriorityQueue<(int Row, int Col, bool NextActionIsOdd), long>();
        frontier.Enqueue(start, 0);

        while (frontier.TryDequeue(out var state, out var distance))
        {
            if (distance > distances[state])
            {
                continue; // A stale entry from an earlier, since-improved relaxation.
            }

            RelaxBcl(state, distance, penalty, (distances, frontier));
        }

        return StartEntranceCost + BestArrival(distances, rowCount, columnCount);
    }

    // One settled state's two kinds of relaxation: staying put, which always costs
    // the cell's own penalty, then every move that is legal from here. The two
    // collections Dijkstra is driven by travel together so neither step has to be
    // handed five separate arguments.
    private static void RelaxBcl(
        (int Row, int Col, bool NextActionIsOdd) state,
        long distance,
        int[][] penalty,
        (Dictionary<(int Row, int Col, bool NextActionIsOdd), long> Distances,
            PriorityQueue<(int Row, int Col, bool NextActionIsOdd), long> Frontier) search)
    {
        var (row, col, nextActionIsOdd) = state;
        var stayPenalty = penalty[row][col];

        Relax((row, col, !nextActionIsOdd), distance + stayPenalty, search.Distances, search.Frontier);
        RelaxNeighborCells((row, col, nextActionIsOdd, stayPenalty), distance, penalty, search);
    }

    // Every move out of one settled state, each paying its destination's entrance cost
    // plus the source cell's own penalty when the move runs against the parity the
    // state is waiting for.
    private static void RelaxNeighborCells(
        (int Row, int Col, bool NextActionIsOdd, int StayPenalty) source,
        long distance,
        int[][] penalty,
        (Dictionary<(int Row, int Col, bool NextActionIsOdd), long> Distances,
            PriorityQueue<(int Row, int Col, bool NextActionIsOdd), long> Frontier) search)
    {
        var rows = penalty.Length;
        var cols = penalty[0].Length;
        var flipped = !source.NextActionIsOdd;

        foreach (var (deltaRow, deltaCol, matchesOddAction) in AlternatingGridMoveTable.Moves)
        {
            var nextRow = source.Row + deltaRow;
            var nextCol = source.Col + deltaCol;

            if (!IsOnGrid(nextRow, nextCol, rows, cols))
            {
                continue;
            }

            var followsParity = matchesOddAction == source.NextActionIsOdd;
            var entrance = (long)(nextRow + 1) * (nextCol + 1);
            var cost = entrance + (followsParity ? 0 : source.StayPenalty);

            Relax((nextRow, nextCol, flipped), distance + cost, search.Distances, search.Frontier);
        }
    }

    // Whether the cell lies on the grid at all.
    private static bool IsOnGrid(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    private static long BestArrival(
        Dictionary<(int Row, int Col, bool NextActionIsOdd), long> distances, int rowCount, int columnCount)
    {
        var viaOdd = distances.GetValueOrDefault((rowCount - 1, columnCount - 1, true), long.MaxValue);
        var viaEven = distances.GetValueOrDefault((rowCount - 1, columnCount - 1, false), long.MaxValue);

        return Math.Min(viaOdd, viaEven);
    }

    public static long MinCostByStateDijkstra(int rowCount, int columnCount, int[][] penalty)
    {
        var source = new AlternatingGridNode(0, 0, NextActionIsOdd: true, penalty);

        var distances = ShortestPath.Dijkstra<
            AlternatingGridNode, AlternatingGridTopology, ListEdges<AlternatingGridNode, long>, long>(source);

        var viaOdd = distances.GetValueOrDefault(
            source with { Row = rowCount - 1, Col = columnCount - 1, NextActionIsOdd = true }, long.MaxValue);
        var viaEven = distances.GetValueOrDefault(
            source with { Row = rowCount - 1, Col = columnCount - 1, NextActionIsOdd = false }, long.MaxValue);

        return StartEntranceCost + Math.Min(viaOdd, viaEven);
    }

    // Relax one state to `candidate` when that beats what is already known for it, and
    // queue it for its own turn. Reached from the waiting step and from every move, so
    // it sits after the two helpers that call it.
    private static void Relax(
        (int Row, int Col, bool NextActionIsOdd) state,
        long candidate,
        Dictionary<(int Row, int Col, bool NextActionIsOdd), long> distances,
        PriorityQueue<(int Row, int Col, bool NextActionIsOdd), long> frontier)
    {
        if (distances.TryGetValue(state, out var known) && candidate >= known)
        {
            return;
        }

        distances[state] = candidate;
        frontier.Enqueue(state, candidate);
    }
}
