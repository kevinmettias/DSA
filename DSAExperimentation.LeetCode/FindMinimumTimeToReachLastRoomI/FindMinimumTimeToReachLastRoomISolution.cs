using DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

namespace DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

// LeetCode 3341. Find Minimum Time to Reach Last Room I: moveTime[r][c] is the
// earliest second a move INTO that room may begin; moving to an orthogonal
// neighbor always costs exactly one second once that move starts. Arriving at a
// neighbor before its moveTime just means waiting first, so the earliest arrival
// is max(currentTime, moveTime[neighbor]) + 1 - a non-negative-weight relaxation
// with a per-edge cost that depends on the caller's current time rather than a
// fixed weight, the same one-degree-more-dynamic shape
// MinimumTimeToVisitACellInAGridSolution documents. That relaxation is
// GridArrivalDijkstra's, declared in LC 2577's folder because that problem states
// what makes a grid search dynamic; this class supplies only the arrival rule,
// WaitForMoveTimeArrivalRule, which is the one place the two problems differ. Unlike
// LC 2577 there is no parity/bounce rule and every room is always reachable eventually
// (waiting is always enough), so the rule has no wait-and-parity branch and there is no
// "no first move exists" precondition to check either.
internal static class FindMinimumTimeToReachLastRoomISolution
{
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

        return GridArrivalDijkstra.ByBclQueue(moveTime, new WaitForMoveTimeArrivalRule());
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

        return GridArrivalDijkstra.ByHeap(moveTime, new WaitForMoveTimeArrivalRule());
    }

    // Earliest arrival at a room requiring `requiredTime`, moving on from `currentTime`: one
    // second to step in, or - if that is still too early - wait where you stand until the
    // room's own moveTime allows the step. The whole of what LC 3341 asks of the shared
    // relaxation; the room is always reachable eventually, so nothing here reports failure.
    private sealed class WaitForMoveTimeArrivalRule : IArrivalRule
    {
        public int Arrive(int currentTime, int requiredTime) =>
            Math.Max(currentTime, requiredTime) + 1;
    }
}
