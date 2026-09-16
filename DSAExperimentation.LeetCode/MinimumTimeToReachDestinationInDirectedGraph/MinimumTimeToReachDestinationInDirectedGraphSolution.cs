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
    public static int MinimumTimeByBclPriorityQueue(int vertexCount, int[][] edges)
    {
        var graph = TimeWindowAdjacency.Build(vertexCount, edges);

        return MinimumTimeByBclPriorityQueue(graph);
    }

    public static int MinimumTimeByBclPriorityQueue(TimeWindowAdjacency graph)
    {
        var destination = graph.Neighbors.Length - 1;

        if (destination == 0)
        {
            return 0;
        }

        var state = NewBclQueueSearchState(graph);

        return SearchByBclQueue(state, graph, destination);
    }

    // The mutable working set of one BCL-queue Dijkstra run: the best arrival
    // time known per node, which nodes have already been settled, and the
    // frontier the unsettled ones wait in.
    private static (
        int[] Earliest,
        bool[] Settled,
        PriorityQueue<int, int> Frontier) NewBclQueueSearchState(TimeWindowAdjacency graph)
    {
        var earliest = NewUnreachableTimes(graph.Neighbors.Length);
        earliest[0] = 0;
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new PriorityQueue<int, int>();
        frontier.Enqueue(0, 0);

        return (earliest, settled, frontier);
    }

    // The relax step for the node just settled: every edge out of it is skipped
    // when its far end is settled or its window cannot be met from `time`, and
    // queued only when it improves the arrival time already known for that far
    // end.
    private static void RelaxNeighborsByBclQueue(
        (int[] Earliest, bool[] Settled, PriorityQueue<int, int> Frontier) state,
        TimeWindowAdjacency graph, int node, int time)
    {
        foreach (var (neighbor, start, end) in graph.Neighbors[node])
        {
            if (state.Settled[neighbor] || !TryArrivalTime(time, start, end, out var arrival))
            {
                continue;
            }

            if (arrival < state.Earliest[neighbor])
            {
                state.Earliest[neighbor] = arrival;
                state.Frontier.Enqueue(neighbor, arrival);
            }
        }
    }

    // Dijkstra over time instead of distance: the frontier is ordered by arrival
    // time, so the first time the destination surfaces is the answer.
    private static int SearchByBclQueue(
        (int[] Earliest, bool[] Settled, PriorityQueue<int, int> Frontier) state,
        TimeWindowAdjacency graph, int destination)
    {
        while (state.Frontier.TryDequeue(out var node, out var time))
        {
            if (state.Settled[node])
            {
                continue;
            }

            state.Settled[node] = true;

            if (node == destination)
            {
                return time;
            }

            RelaxNeighborsByBclQueue(state, graph, node, time);
        }

        return LeetCodeAnswer.None;
    }

    // Composed: identical algorithm, fronted by this repo's own
    // Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
    // frontier ShortestPath.Dijkstra/AStar and every other dynamic-relaxation
    // Dijkstra coverage problem in this repo already use.
    public static int MinimumTimeByHeap(int vertexCount, int[][] edges)
    {
        var graph = TimeWindowAdjacency.Build(vertexCount, edges);

        return MinimumTimeByHeap(graph);
    }

    public static int MinimumTimeByHeap(TimeWindowAdjacency graph)
    {
        var destination = graph.Neighbors.Length - 1;

        if (destination == 0)
        {
            return 0;
        }

        var state = NewHeapSearchState(graph);

        return SearchByHeap(state, graph, destination);
    }

    // The same working set as NewBclQueueSearchState, over this repo's own
    // priority queue.
    private static (
        int[] Earliest,
        bool[] Settled,
        Heap<(int Node, int Time), ByPriorityOrder<int, int>> Frontier) NewHeapSearchState(TimeWindowAdjacency graph)
    {
        var earliest = NewUnreachableTimes(graph.Neighbors.Length);
        earliest[0] = 0;
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new Heap<(int Node, int Time), ByPriorityOrder<int, int>>();
        frontier.Push((0, 0));

        return (earliest, settled, frontier);
    }

    private static void RelaxNeighborsByHeap(
        (int[] Earliest, bool[] Settled, Heap<(int Node, int Time), ByPriorityOrder<int, int>> Frontier) state,
        TimeWindowAdjacency graph, int node, int time)
    {
        foreach (var (neighbor, start, end) in graph.Neighbors[node])
        {
            if (state.Settled[neighbor] || !TryArrivalTime(time, start, end, out var arrival))
            {
                continue;
            }

            if (arrival < state.Earliest[neighbor])
            {
                state.Earliest[neighbor] = arrival;
                state.Frontier.Push((neighbor, arrival));
            }
        }
    }

    private static int SearchByHeap(
        (int[] Earliest, bool[] Settled, Heap<(int Node, int Time), ByPriorityOrder<int, int>> Frontier) state,
        TimeWindowAdjacency graph, int destination)
    {
        while (state.Frontier.TryPop(out var entry))
        {
            var (node, time) = entry;

            if (state.Settled[node])
            {
                continue;
            }

            state.Settled[node] = true;

            if (node == destination)
            {
                return time;
            }

            RelaxNeighborsByHeap(state, graph, node, time);
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

    private static int[] NewUnreachableTimes(int vertexCount)
    {
        var times = new int[vertexCount];
        Array.Fill(times, int.MaxValue);
        return times;
    }
}
