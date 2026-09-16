using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

// LeetCode 3377. Digit Operations to Make Two Integers Equal: startValue's cost
// to become targetValue is the sum of every value it passes through (startValue
// itself included), moving one digit by +-1 per step, while the value must never
// be prime and must never change digit count. That is a shortest-path query -
// "the cost of an edge is the value you land on" - over DigitStepGraph, the
// non-prime digit-mutation graph.
internal static class DigitOperationsToMakeTwoIntegersEqualSolution
{
    // The textbook priority-queue Dijkstra: BCL PriorityQueue + Dictionary,
    // generating each candidate mutation on the fly and never materializing
    // the graph - the arm the composed strategy below has to justify itself
    // against. Still calls DigitStepGraph's own DigitMutations/IsPrime for
    // the problem's digit-move and primality rules, the same "reuse the
    // domain's own neighbor rule, keep only the search engine textbook" split
    // OpenTheLockSolution's MinTurnsByMutationQueue makes with LockGraph.
    public static int MinOperationsByBruteForceDijkstra(int startValue, int targetValue)
    {
        if (DigitStepGraph.IsPrime(startValue) || DigitStepGraph.IsPrime(targetValue))
        {
            return LeetCodeAnswer.None;
        }

        var best = new Dictionary<int, int> { [startValue] = startValue };
        var queue = new PriorityQueue<int, int>();
        queue.Enqueue(startValue, startValue);

        return CostByMutationQueue(targetValue, best, queue);
    }

    // The search itself: settle the nearest unsettled value, then relax its digit
    // mutations. A stale queue entry - one whose recorded cost has since been beaten -
    // is dropped rather than re-expanded.
    private static int CostByMutationQueue(int targetValue, Dictionary<int, int> best, PriorityQueue<int, int> queue)
    {
        while (queue.TryDequeue(out var current, out var cost))
        {
            if (current == targetValue)
            {
                return cost;
            }

            if (cost > best[current])
            {
                continue;
            }

            RelaxMutations(current, cost, best, queue);
        }

        return LeetCodeAnswer.None;
    }

    private static void RelaxMutations(int current, int cost, Dictionary<int, int> best, PriorityQueue<int, int> queue)
    {
        foreach (var neighbor in DigitStepGraph.DigitMutations(current))
        {
            if (DigitStepGraph.IsPrime(neighbor))
            {
                continue;
            }

            var candidate = cost + neighbor;

            if (!best.TryGetValue(neighbor, out var known) || candidate < known)
            {
                best[neighbor] = candidate;
                queue.Enqueue(neighbor, candidate);
            }
        }
    }

    // This repo's own Dijkstra: ShortestPath.Dijkstra over DigitStepNode via
    // DigitStepTopology gives every reachable value's distance-in-value-sum
    // from startValue in one call; adding startValue itself back in (the source's
    // own value, which Dijkstra's Distances[source] = Zero convention never
    // charges) gives LC 3377's own cost definition.
    public static int MinOperationsByDijkstraOverDigitGraph(int startValue, int targetValue) =>
        MinOperationsByDijkstraOverDigitGraph(
            DigitStepGraph.Build(startValue.ToString().Length), startValue, targetValue);

    public static int MinOperationsByDijkstraOverDigitGraph(DigitStepGraph graph, int startValue, int targetValue)
    {
        if (!graph.Nodes.TryGetValue(startValue, out var startNode)
            || !graph.Nodes.TryGetValue(targetValue, out var targetNode))
        {
            return LeetCodeAnswer.None;
        }

        var distances = ShortestPath
            .Dijkstra<DigitStepNode, DigitStepTopology, ListEdges<DigitStepNode, int>, int>(startNode);

        return distances.TryGetValue(targetNode, out var distance)
            ? CostIncludingSource(startValue, distance)
            : LeetCodeAnswer.None;
    }

    // The source node's own value is never charged by Dijkstra's Distances[source] = Zero
    // convention, so the reported cost is its value plus the distance the search found.
    private static int CostIncludingSource(int source, int distance) => source + distance;
}
