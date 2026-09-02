using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumTimeToReachDestinationInDirectedGraph;

// LeetCode 3604. Minimum Time to Reach Destination in Directed Graph: edge
// (u, v, start, end) may only be crossed at some integer time t with
// start <= t <= end, and waiting in place costs one unit of time per unit
// waited. The earliest time a traveler standing at u at time `now` can cross to
// v is therefore max(now, start) + 1, unless that would exceed end, in which case
// the edge simply cannot be used from `now` at all - a per-edge cost that depends
// on the caller's current time rather than a fixed weight, one degree more
// dynamic than ShortestPath.Dijkstra's IEdgeTopology (a fixed weight per edge) can
// express, the same gap FindMinimumTimeToReachLastRoomISolution's own doc comment
// names. Both arms below hand-roll the same relax loop, one off a priority queue
// this repo doesn't own, one off the priority queue it does - the same split
// MinimumTimeToVisitDisappearingNodesSolution uses for the same reason.
internal static class MinimumTimeToReachDestinationInDirectedGraphSolution
{
    // Baseline: BCL PriorityQueue<int,int> - "what you'd write without this repo"
    // (ARCHITECTURE.md 17.5).
    public static int MinimumTimeByBclPriorityQueue(int n, int[][] edges) =>
        MinimumTimeByBclPriorityQueue(TimeWindowAdjacency.Build(n, edges));

    public static int MinimumTimeByBclPriorityQueue(TimeWindowAdjacency graph)
    {
        var destination = graph.Neighbors.Length - 1;

        if (destination == 0)
        {
            return 0;
        }

        var earliest = NewUnreachableTimes(graph.Neighbors.Length);
        earliest[0] = 0;
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new PriorityQueue<int, int>();
        frontier.Enqueue(0, 0);

        while (frontier.TryDequeue(out var node, out var time))
        {
            if (settled[node])
            {
                continue;
            }

            settled[node] = true;

            if (node == destination)
            {
                return time;
            }

            foreach (var (neighbor, start, end) in graph.Neighbors[node])
            {
                if (settled[neighbor] || !TryArrivalTime(time, start, end, out var arrival))
                {
                    continue;
                }

                if (arrival < earliest[neighbor])
                {
                    earliest[neighbor] = arrival;
                    frontier.Enqueue(neighbor, arrival);
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // Composed: identical algorithm, fronted by this repo's own
    // Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
    // frontier ShortestPath.Dijkstra/AStar and every other dynamic-relaxation
    // Dijkstra coverage problem in this repo already use.
    public static int MinimumTimeByHeap(int n, int[][] edges) =>
        MinimumTimeByHeap(TimeWindowAdjacency.Build(n, edges));

    public static int MinimumTimeByHeap(TimeWindowAdjacency graph)
    {
        var destination = graph.Neighbors.Length - 1;

        if (destination == 0)
        {
            return 0;
        }

        var earliest = NewUnreachableTimes(graph.Neighbors.Length);
        earliest[0] = 0;
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new Heap<(int Node, int Time), ByPriorityOrder<int, int>>();
        frontier.Push((0, 0));

        while (frontier.TryPop(out var entry))
        {
            var (node, time) = entry;

            if (settled[node])
            {
                continue;
            }

            settled[node] = true;

            if (node == destination)
            {
                return time;
            }

            foreach (var (neighbor, start, end) in graph.Neighbors[node])
            {
                if (settled[neighbor] || !TryArrivalTime(time, start, end, out var arrival))
                {
                    continue;
                }

                if (arrival < earliest[neighbor])
                {
                    earliest[neighbor] = arrival;
                    frontier.Push((neighbor, arrival));
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // The earliest a traveler present at `now` can cross this window: wait (if
    // needed) until `start`, then one more unit to make the crossing - unless
    // that departure would already be past `end`, in which case the edge cannot
    // be used from `now` at all.
    private static bool TryArrivalTime(int now, int start, int end, out int arrival)
    {
        var departure = Math.Max(now, start);

        if (departure > end)
        {
            arrival = default;
            return false;
        }

        arrival = departure + 1;
        return true;
    }

    private static int[] NewUnreachableTimes(int n)
    {
        var times = new int[n];
        Array.Fill(times, int.MaxValue);
        return times;
    }
}
