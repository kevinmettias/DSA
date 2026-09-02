using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

// LeetCode 3594. Minimum Time to Transport All Individuals: a crossing, and
// (when the crossing didn't finish everyone) the return trip it forces, form
// one combined round-trip move over the state "who has already arrived, what
// environmental stage the crossing happens under" - a shortest-path query
// over TransportGraph, the same "the puzzle IS a graph search, only the
// neighbor rule is domain-specific" shape OpenTheLock and
// DigitOperationsToMakeTwoIntegersEqual both use. Every edge weight is a
// crossing or return time, at least time[i] x 0.5 > 0, so the graph - which
// can genuinely cycle back to a state it has already visited, most plainly
// when capacity is 1 and the same person must ferry the boat back and forth
// forever - stays sound for Dijkstra despite never being a DAG; a memoized
// top-down recurrence (Algorithms.DynamicProgramming.Memoizer) would not be,
// since it requires the state graph to be well-founded.
internal static class MinimumTimeToTransportAllIndividualsSolution
{
    // Textbook baseline: BCL PriorityQueue<(mask,stage), double> Dijkstra,
    // reusing TransportGraph.Rounds for the pure river-crossing rule (the
    // same "reuse the domain's own neighbor rule, keep only the search engine
    // textbook" split OpenTheLockSolution's MinTurnsByMutationQueue makes
    // with LockGraph.WheelTurnNeighbors) but never materializing the graph
    // itself. The arm the composed strategy below has to justify itself
    // against.
    public static double MinTimeByBruteForceDijkstra(int[] time, int capacity, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;
        var best = new Dictionary<(int Mask, int Stage), double> { [(0, 0)] = 0 };
        var queue = new PriorityQueue<(int Mask, int Stage), double>();
        queue.Enqueue((0, 0), 0);

        while (queue.TryDequeue(out var state, out var cost))
        {
            if (cost > best[state])
            {
                continue;
            }

            if (state.Mask == fullMask)
            {
                return cost;
            }

            RelaxRounds(state, cost, time, capacity, mul, fullMask, best, queue);
        }

        return LeetCodeAnswer.None;
    }

    private static void RelaxRounds(
        (int Mask, int Stage) state, double cost, int[] time, int capacity, double[] mul, int fullMask,
        Dictionary<(int Mask, int Stage), double> best, PriorityQueue<(int Mask, int Stage), double> queue)
    {
        foreach (var (roundTime, nextMask, nextStage) in
                 TransportGraph.Rounds(state.Mask, state.Stage, time, capacity, mul, fullMask))
        {
            var candidate = cost + roundTime;
            var next = (nextMask, nextStage);

            if (!best.TryGetValue(next, out var known) || candidate < known)
            {
                best[next] = candidate;
                queue.Enqueue(next, candidate);
            }
        }
    }

    // This repo's own Dijkstra: ShortestPath.Dijkstra over TransportState via
    // TransportTopology gives every reachable (mask, stage)'s distance from
    // the start in one call; the answer is the cheapest of those whose mask
    // is full, since the puzzle may finish under any of the m stages.
    public static double MinTimeByDijkstraOverTransportGraph(int[] time, int capacity, double[] mul) =>
        MinTimeByDijkstraOverTransportGraph(TransportGraph.Build(time, capacity, mul));

    public static double MinTimeByDijkstraOverTransportGraph(TransportGraph graph)
    {
        var distances = ShortestPath
            .Dijkstra<TransportState, TransportTopology, ListEdges<TransportState, double>, double>(graph.Source);

        var best = double.PositiveInfinity;

        foreach (var (node, distance) in distances)
        {
            if (node.Mask == graph.FullMask && distance < best)
            {
                best = distance;
            }
        }

        return double.IsPositiveInfinity(best) ? LeetCodeAnswer.None : best;
    }
}
