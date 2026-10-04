using DSAExperimentation.Algorithms.ShortestPaths.Grids;

namespace DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

// LeetCode 3341. Find Minimum Time to Reach Last Room I: moveTime[r][c] is the
// earliest second a move INTO that room may begin; moving to an orthogonal
// neighbor always costs exactly one second once that move starts. Arriving at a
// neighbor before its moveTime just means waiting first, so the earliest arrival
// is max(currentTime, moveTime[neighbor]) + 1 - a non-negative-weight relaxation
// with a per-edge cost that depends on the caller's current time rather than a
// fixed weight: the time-dependent grid search
// Algorithms.ShortestPaths.Grids.GridEarliestArrival runs, priced here by
// WaitInPlaceArrival. Unlike LC 2577 there is no parity bounce and every room is
// always reachable eventually (waiting is always enough), so there is no
// "no first move exists" precondition to check.
internal static class FindMinimumTimeToReachLastRoomISolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // Baseline: the textbook Dijkstra over the rooms - a BCL PriorityQueue frontier,
    // a Dictionary of best arrivals and a HashSet of settled rooms, with the wait
    // written inline - "what you'd write without this repo" (ARCHITECTURE.md 17.5).
    public static int MinimumTimeByBclPriorityQueue(int[][] moveTime)
    {
        var search = new RoomSearch(
            moveTime, new() { [(0, 0)] = 0 }, [], new PriorityQueue<(int Row, int Col), int>());
        var lastRoom = (moveTime.Length - 1, moveTime[0].Length - 1);
        search.Frontier.Enqueue((0, 0), 0);

        while (search.Frontier.TryDequeue(out var room, out var time))
        {
            if (room == lastRoom)
            {
                return time;
            }

            if (search.Settled.Add(room))
            {
                OfferNeighbours(search, room, time);
            }
        }

        return LeetCodeAnswer.None;
    }

    // Every neighbouring room the move reaches sooner than any route found so far: wait
    // where you stand until its moveTime, then take the one second the move costs.
    private static void OfferNeighbours(RoomSearch search, (int Row, int Col) room, int time)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: room.Row + dRow, Col: room.Col + dCol);

            if (!IsRoom(search.MoveTime, next))
            {
                continue;
            }

            var arrival = Math.Max(time, search.MoveTime[next.Row][next.Col]) + 1;

            if (!search.Best.TryGetValue(next, out var known) || arrival < known)
            {
                search.Best[next] = arrival;
                search.Frontier.Enqueue(next, arrival);
            }
        }
    }

    // Whether the coordinates name a room of the grid at all.
    private static bool IsRoom(int[][] moveTime, (int Row, int Col) cell) =>
        cell.Row >= 0 && cell.Row < moveTime.Length && cell.Col >= 0 && cell.Col < moveTime[0].Length;

    // Composed: GridEarliestArrival, this repo's time-dependent grid search over its own
    // Heap, priced by WaitInPlaceArrival.
    public static int MinimumTimeByHeap(int[][] moveTime) =>
        GridEarliestArrival.Time<WaitInPlaceArrival>(moveTime, (0, 0), (moveTime.Length - 1, moveTime[0].Length - 1));

    // One search's state: the rooms' move times, each room's best arrival so far, the rooms
    // already settled, and the queue of pending arrivals.
    private sealed record RoomSearch(
        int[][] MoveTime,
        Dictionary<(int Row, int Col), int> Best,
        HashSet<(int Row, int Col)> Settled,
        PriorityQueue<(int Row, int Col), int> Frontier);
}
