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

        while (frontier.TryDequeue(out var node, out var priority))
        {
            if (!settled.Add(node))
            {
                continue;
            }

            if (node.Row == rows - 1 && node.Col == cols - 1)
            {
                return priority;
            }

            foreach (var neighbor in Neighbors(node, rows, cols))
            {
                var arrival = ArrivalTime(priority, moveTime[neighbor.Row][neighbor.Col], node.Parity);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Enqueue(neighbor, arrival);
                }
            }
        }

        return Unreachable(rows, cols);
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

        while (frontier.TryPop(out var entry))
        {
            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == rows - 1 && entry.Node.Col == cols - 1)
            {
                return entry.Priority;
            }

            foreach (var neighbor in Neighbors(entry.Node, rows, cols))
            {
                var arrival = ArrivalTime(entry.Priority, moveTime[neighbor.Row][neighbor.Col], entry.Node.Parity);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Push((neighbor, arrival));
                }
            }
        }

        return Unreachable(rows, cols);
    }

    private static IEnumerable<(int Row, int Col, int Parity)> Neighbors(
        (int Row, int Col, int Parity) node, int rows, int cols)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var (nextRow, nextCol) = (node.Row + dRow, node.Col + dCol);

            if (nextRow >= 0 && nextRow < rows && nextCol >= 0 && nextCol < cols)
            {
                yield return (nextRow, nextCol, 1 - node.Parity);
            }
        }
    }

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
