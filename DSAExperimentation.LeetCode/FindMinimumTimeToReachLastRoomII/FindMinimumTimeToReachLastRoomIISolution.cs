using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomII;

// LeetCode 3342. Find Minimum Time to Reach Last Room II: moveTime[i][j] is the
// earliest second room (i, j) may be entered, and successive moves alternate
// costing one then two seconds (first move 1s, second 2s, third 1s, ...).
// Reaching a neighbor therefore costs
// max(currentTime, moveTime[neighbor]) + (nextMoveIsOdd ? 1 : 2) - the same
// dynamic-wait relaxation as LC 2577 (MinimumTimeToVisitACellInAGridSolution),
// one degree more dynamic than ShortestPath.Dijkstra's IEdgeTopology (a fixed
// weight per edge) can express, plus a second axis LC 2577 didn't have: the
// move's own cost alternates with how many moves have been taken, so the search
// state is (row, col, parity) rather than just (row, col). Both arms compose
// Collections.Heap<Element,TOrder>/BCL PriorityQueue directly with a bespoke
// relaxation loop, the same precedent MinimumTimeToVisitACellInAGridSolution
// already set for this exact shape of problem.
internal static class FindMinimumTimeToReachLastRoomIISolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // Baseline: "what you'd write without this repo" (§17.5) - BCL PriorityQueue.
    public static int MinTimeByBclPriorityQueue(int[][] moveTime)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);
        var distances = new Dictionary<(int Row, int Col, int Parity), int> { [(0, 0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col, int Parity)>();
        var frontier = new PriorityQueue<(int Row, int Col, int Parity), int>();
        frontier.Enqueue((0, 0, 0), 0);

        return DrainFrontierByBclPriorityQueue((moveTime, rows, cols), distances, settled, frontier);
    }

    // The relaxation itself, over the BCL frontier and the tables its caller seeded:
    // pop the cheapest unsettled state, answer with its time when it is the last room,
    // and otherwise offer every neighbour the arrival time this move reaches it at.
    private static int DrainFrontierByBclPriorityQueue(
        (int[][] MoveTime, int Rows, int Cols) grid,
        Dictionary<(int Row, int Col, int Parity), int> distances,
        HashSet<(int Row, int Col, int Parity)> settled,
        PriorityQueue<(int Row, int Col, int Parity), int> frontier)
    {
        while (frontier.TryDequeue(out var node, out var priority))
        {
            if (!settled.Add(node))
            {
                continue;
            }

            if (node.Row == grid.Rows - 1 && node.Col == grid.Cols - 1)
            {
                return priority;
            }

            foreach (var neighbor in Neighbors(node, grid.Rows, grid.Cols))
            {
                var arrival = ArrivalTime(priority, grid.MoveTime[neighbor.Row][neighbor.Col], node.Parity);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Enqueue(neighbor, arrival);
                }
            }
        }

        return Unreachable(grid.Rows, grid.Cols);
    }

    // Composed: identical algorithm, fronted by Collections.Heap<Element,TOrder>
    // ordered by ByPriorityOrder<TNode,TWeight> - the same production heap
    // ShortestPath.Dijkstra/AStar and every other grid-Dijkstra coverage test in
    // this repo already use as their frontier.
    public static int MinTimeByHeap(int[][] moveTime)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);
        var distances = new Dictionary<(int Row, int Col, int Parity), int> { [(0, 0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col, int Parity)>();
        var frontier = new Heap<
            ((int Row, int Col, int Parity) Node, int Priority),
            ByPriorityOrder<(int Row, int Col, int Parity), int>>();
        frontier.Push(((0, 0, 0), 0));

        return DrainFrontierByHeap((moveTime, rows, cols), distances, settled, frontier);
    }

    // The same relaxation over this repo's Heap instead, whose pop hands the node and
    // its priority back as one entry rather than through two out parameters.
    private static int DrainFrontierByHeap(
        (int[][] MoveTime, int Rows, int Cols) grid,
        Dictionary<(int Row, int Col, int Parity), int> distances,
        HashSet<(int Row, int Col, int Parity)> settled,
        Heap<((int Row, int Col, int Parity) Node, int Priority), ByPriorityOrder<(int Row, int Col, int Parity), int>> frontier)
    {
        while (frontier.TryPop(out var entry))
        {
            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == grid.Rows - 1 && entry.Node.Col == grid.Cols - 1)
            {
                return entry.Priority;
            }

            foreach (var neighbor in Neighbors(entry.Node, grid.Rows, grid.Cols))
            {
                var arrival = ArrivalTime(entry.Priority, grid.MoveTime[neighbor.Row][neighbor.Col], entry.Node.Parity);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Push((neighbor, arrival));
                }
            }
        }

        return Unreachable(grid.Rows, grid.Cols);
    }

    private static IEnumerable<(int Row, int Col, int Parity)> Neighbors(
        (int Row, int Col, int Parity) node, int rows, int cols)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var (nextRow, nextCol) = (node.Row + dRow, node.Col + dCol);

            if (IsInsideGrid(nextRow, nextCol, rows, cols))
            {
                yield return (nextRow, nextCol, 1 - node.Parity);
            }
        }
    }

    // Whether the neighbor stays on the board.
    private static bool IsInsideGrid(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    // Arrival time when leaving `currentTime` for a room requiring `requiredTime`:
    // wait for the room to open if necessary, then pay the alternating move cost -
    // one second for an odd-numbered move (fromParity == 0), two for an even one.
    private static int ArrivalTime(int currentTime, int requiredTime, int fromParity)
    {
        var cost = fromParity == 0 ? 1 : 2;
        return Math.Max(currentTime, requiredTime) + cost;
    }

    // Every room is orthogonally reachable and moveTime only ever delays entry,
    // never blocks it, so the last room is always settled before the frontier
    // empties - this path exists only to satisfy the compiler's return analysis.
    private static int Unreachable(int rows, int cols) =>
        throw new InvalidOperationException($"Room ({rows - 1}, {cols - 1}) is unreachable - not possible per LC 3342's constraints.");
}
