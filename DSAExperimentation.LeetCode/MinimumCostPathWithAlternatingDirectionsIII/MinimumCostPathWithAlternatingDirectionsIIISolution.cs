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

    private static readonly (int DeltaRow, int DeltaCol, bool MatchesOddAction)[] Moves =
    [
        (1, 0, true), (0, 1, true),
        (-1, 0, false), (0, -1, false),
    ];

    public static long MinCostByBclDijkstra(int m, int n, int[][] penalty)
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

            RelaxBcl(state, distance, penalty, distances, frontier);
        }

        return StartEntranceCost + BestArrival(distances, m, n);
    }

    private static void RelaxBcl(
        (int Row, int Col, bool NextActionIsOdd) state,
        long distance,
        int[][] penalty,
        Dictionary<(int Row, int Col, bool NextActionIsOdd), long> distances,
        PriorityQueue<(int Row, int Col, bool NextActionIsOdd), long> frontier)
    {
        var (row, col, nextActionIsOdd) = state;
        var rows = penalty.Length;
        var cols = penalty[0].Length;
        var stayPenalty = penalty[row][col];
        var flipped = !nextActionIsOdd;

        Relax((row, col, flipped), distance + stayPenalty, distances, frontier);

        foreach (var (deltaRow, deltaCol, matchesOddAction) in Moves)
        {
            var nextRow = row + deltaRow;
            var nextCol = col + deltaCol;

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
            {
                continue;
            }

            var followsParity = matchesOddAction == nextActionIsOdd;
            var entrance = (long)(nextRow + 1) * (nextCol + 1);
            var cost = entrance + (followsParity ? 0 : stayPenalty);

            Relax((nextRow, nextCol, flipped), distance + cost, distances, frontier);
        }
    }

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

    private static long BestArrival(
        Dictionary<(int Row, int Col, bool NextActionIsOdd), long> distances, int m, int n)
    {
        var viaOdd = distances.GetValueOrDefault((m - 1, n - 1, true), long.MaxValue);
        var viaEven = distances.GetValueOrDefault((m - 1, n - 1, false), long.MaxValue);

        return Math.Min(viaOdd, viaEven);
    }

    public static long MinCostByStateDijkstra(int m, int n, int[][] penalty)
    {
        var source = new AlternatingGridNode(0, 0, NextActionIsOdd: true, penalty);

        var distances = ShortestPath.Dijkstra<
            AlternatingGridNode, AlternatingGridTopology, ListEdges<AlternatingGridNode, long>, long>(source);

        var viaOdd = distances.GetValueOrDefault(
            source with { Row = m - 1, Col = n - 1, NextActionIsOdd = true }, long.MaxValue);
        var viaEven = distances.GetValueOrDefault(
            source with { Row = m - 1, Col = n - 1, NextActionIsOdd = false }, long.MaxValue);

        return StartEntranceCost + Math.Min(viaOdd, viaEven);
    }
}
