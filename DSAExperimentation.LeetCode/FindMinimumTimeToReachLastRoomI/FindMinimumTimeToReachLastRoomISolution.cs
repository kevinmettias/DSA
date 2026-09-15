using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

// LeetCode 3341. Find Minimum Time to Reach Last Room I: moveTime[r][c] is the
// earliest second a move INTO that room may begin; moving to an orthogonal
// neighbor always costs exactly one second once that move starts. Arriving at a
// neighbor before its moveTime just means waiting first, so the earliest arrival
// is max(currentTime, moveTime[neighbor]) + 1 - a non-negative-weight relaxation
// with a per-edge cost that depends on the caller's current time rather than a
// fixed weight, the same one-degree-more-dynamic shape
// MinimumTimeToVisitACellInAGridSolution documents (composes Heap<Element,TOrder>
// and Graph.ShortestPaths' ByPriorityOrder directly instead of
// ShortestPath.Dijkstra's fixed-weight IEdgeTopology, which cannot express a cost
// that depends on the settled time you're relaxing from). Unlike LC 2577 there is
// no parity/bounce rule and every room is always reachable eventually (waiting is
// always enough), so ArrivalTime has no wait-and-parity branch and there is no
// "no first move exists" precondition to check either.
internal static class FindMinimumTimeToReachLastRoomISolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // Baseline: the same relaxation, fronted by the BCL's own
    // PriorityQueue<TElement,TPriority> instead of this repo's Heap - "what you'd
    // write without this repo" (ARCHITECTURE.md 17.5).
    public static int MinimumTimeByBclPriorityQueue(int[][] moveTime)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);

        if (rows == 1 && cols == 1)
        {
            return 0;
        }

        return SearchByBclQueue(moveTime, (rows, cols));
    }

    // The search itself: settle rooms in nondecreasing arrival order until the last
    // room comes off the BCL queue - its priority is then final - and report -1 if
    // the queue runs dry first.
    private static int SearchByBclQueue(int[][] moveTime, (int Rows, int Cols) size)
    {
        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new PriorityQueue<(int Row, int Col), int>();
        frontier.Enqueue((0, 0), 0);

        while (frontier.TryDequeue(out var node, out var priority))
        {
            if (!settled.Add(node))
            {
                continue;
            }

            if (node.Row == size.Rows - 1 && node.Col == size.Cols - 1)
            {
                return priority;
            }

            RelaxBclNeighbors(moveTime, (node, priority), distances, frontier);
        }

        return -1;
    }

    // Relaxes every neighbor of the room just settled: an arrival earlier than the
    // one already recorded replaces it and re-enters the queue.
    private static void RelaxBclNeighbors(
        int[][] moveTime, ((int Row, int Col) Node, int Priority) entry,
        Dictionary<(int Row, int Col), int> distances, PriorityQueue<(int Row, int Col), int> frontier)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);

        foreach (var neighbor in Neighbors(entry.Node, rows, cols))
        {
            var arrival = ArrivalTime(entry.Priority, moveTime[neighbor.Row][neighbor.Col]);

            if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
            {
                distances[neighbor] = arrival;
                frontier.Enqueue(neighbor, arrival);
            }
        }
    }

    // Composed: identical algorithm, fronted by this repo's own
    // Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
    // frontier ShortestPath.Dijkstra/AStar and MinimumTimeToVisitACellInAGrid's own
    // composed arm already use.
    public static int MinimumTimeByHeap(int[][] moveTime)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);

        if (rows == 1 && cols == 1)
        {
            return 0;
        }

        return SearchByHeap(moveTime, (rows, cols));
    }

    // The same search over this repo's own frontier, whose popped entry already
    // carries the node and its priority together.
    private static int SearchByHeap(int[][] moveTime, (int Rows, int Cols) size)
    {
        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == size.Rows - 1 && entry.Node.Col == size.Cols - 1)
            {
                return entry.Priority;
            }

            RelaxHeapNeighbors(moveTime, entry, distances, frontier);
        }

        return -1;
    }

    private static void RelaxHeapNeighbors(
        int[][] moveTime, ((int Row, int Col) Node, int Priority) entry,
        Dictionary<(int Row, int Col), int> distances,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        var (rows, cols) = (moveTime.Length, moveTime[0].Length);

        foreach (var neighbor in Neighbors(entry.Node, rows, cols))
        {
            var arrival = ArrivalTime(entry.Priority, moveTime[neighbor.Row][neighbor.Col]);

            if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
            {
                distances[neighbor] = arrival;
                frontier.Push((neighbor, arrival));
            }
        }
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) node, int rows, int cols)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: node.Row + dRow, Col: node.Col + dCol);

            if (IsInside(next.Row, next.Col, rows, cols))
            {
                yield return next;
            }
        }
    }

    // Both coordinates within the room grid is one idea, tested as a pair on all
    // four edges.
    private static bool IsInside(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    // Earliest arrival at a room requiring `requiredTime`, moving on from
    // `currentTime`: one second to step in, or - if that is still too early - wait
    // where you stand until the room's own moveTime allows the step.
    private static int ArrivalTime(int currentTime, int requiredTime) =>
        Math.Max(currentTime, requiredTime) + 1;
}
