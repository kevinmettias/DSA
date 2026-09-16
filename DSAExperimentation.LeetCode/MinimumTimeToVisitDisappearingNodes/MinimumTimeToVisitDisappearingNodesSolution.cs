using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;

namespace DSAExperimentation.LeetCode.MinimumTimeToVisitDisappearingNodes;

// LeetCode 3112. Minimum Time to Visit Disappearing Nodes: node i can only be
// occupied while time < disappear[i], so the earliest a node can legally be
// visited is exactly its ordinary Dijkstra distance UNLESS that distance itself
// already fails the node's own deadline - in which case the node is permanently
// dead, and worse, unusable as a waypoint for anything past it, since any path
// through it would have to arrive at least that late. Settling nodes in
// increasing distance order (Dijkstra's own invariant) is exactly what lets each
// node's aliveness be decided the instant it settles, before a too-late arrival
// can leak into anything relaxed from it. That per-settle deadline check has no
// hook in Algorithms.ShortestPaths.ShortestPath.Dijkstra, which always expands
// every settled node - so both strategies here hand-roll the same relax loop,
// one off a priority queue this repo doesn't own, one off the priority queue it
// does.
internal static class MinimumTimeToVisitDisappearingNodesSolution
{
    // Baseline: BCL PriorityQueue<int,int> - "what you'd write without this repo".
    public static int[] MinimumTimesByDijkstraQueue(int nodeCount, int[][] edges, int[] disappear)
    {
        var graph = TimedAdjacency.Build(nodeCount, edges);

        return MinimumTimesByDijkstraQueue(graph, disappear);
    }

    public static int[] MinimumTimesByDijkstraQueue(TimedAdjacency graph, int[] disappear)
    {
        var distances = NewUnreachableDistances(graph.Neighbors.Length);
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new PriorityQueue<int, int>();
        frontier.Enqueue(0, 0);

        SettleAndRelaxByQueue(graph, disappear, frontier, (settled, distances));

        return distances;
    }

    // The one settle/relax loop both arms share, parameterized on the frontier: a node
    // pops with its best-known arrival time, is dropped when it is stale or already
    // past its own deadline, and otherwise settles at that time and offers every
    // unsettled neighbor back to the frontier.
    private static void SettleAndRelaxByQueue(
        TimedAdjacency graph, int[] disappear, PriorityQueue<int, int> frontier,
        (bool[] Settled, int[] Distances) state)
    {
        while (frontier.TryDequeue(out var node, out var time))
        {
            if (state.Settled[node] || time >= disappear[node])
            {
                continue;
            }

            state.Settled[node] = true;
            state.Distances[node] = time;

            foreach (var (neighbor, weight) in graph.Neighbors[node])
            {
                if (!state.Settled[neighbor])
                {
                    frontier.Enqueue(neighbor, time + weight);
                }
            }
        }
    }

    // Composed: this repo's own Heap<T,TOrder> as the frontier, ordered by
    // Graph.ShortestPaths' ByPriorityOrder witness - the same priority-queue
    // primitive ShortestPath.Dijkstra itself is built from (§7's "prefer an
    // already-existing self-hosted structure over its BCL equivalent"), just with
    // the settle-time deadline check threaded through the relax loop by hand.
    public static int[] MinimumTimesByPriorityHeap(int nodeCount, int[][] edges, int[] disappear)
    {
        var graph = TimedAdjacency.Build(nodeCount, edges);

        return MinimumTimesByPriorityHeap(graph, disappear);
    }

    public static int[] MinimumTimesByPriorityHeap(TimedAdjacency graph, int[] disappear)
    {
        var distances = NewUnreachableDistances(graph.Neighbors.Length);
        var settled = new bool[graph.Neighbors.Length];
        var frontier = new Heap<(int Node, int Time), ByPriorityOrder<int, int>>();
        frontier.Push((0, 0));

        SettleAndRelaxByHeap(graph, disappear, frontier, (settled, distances));

        return distances;
    }

    // The same loop as SettleAndRelaxByQueue against this repo's own Heap frontier;
    // only the pop and push calls differ, the deadline check and the relax order are
    // identical.
    private static void SettleAndRelaxByHeap(
        TimedAdjacency graph, int[] disappear,
        Heap<(int Node, int Time), ByPriorityOrder<int, int>> frontier,
        (bool[] Settled, int[] Distances) state)
    {
        while (frontier.TryPop(out var entry))
        {
            var (node, time) = entry;

            if (state.Settled[node] || time >= disappear[node])
            {
                continue;
            }

            state.Settled[node] = true;
            state.Distances[node] = time;

            foreach (var (neighbor, weight) in graph.Neighbors[node])
            {
                if (!state.Settled[neighbor])
                {
                    frontier.Push((neighbor, time + weight));
                }
            }
        }
    }

    private static int[] NewUnreachableDistances(int nodeCount)
    {
        var distances = new int[nodeCount];
        Array.Fill(distances, LeetCodeAnswer.None);
        return distances;
    }
}
