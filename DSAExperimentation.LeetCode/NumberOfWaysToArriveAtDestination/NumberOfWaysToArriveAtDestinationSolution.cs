using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

// LeetCode 1976. Number of Ways to Arrive at Destination: n intersections joined by
// bi-directional roads with positive travel times. Count the journeys from
// intersection 0 to intersection n - 1 that take the shortest possible time,
// modulo 1e9+7.
//
// Both strategies share WaysGraph's single Dijkstra run, sourced at the destination
// so every intersection's Dist is its own shortest time to it - the distances are
// what "on a shortest path" is stated against, not part of counting - so what
// separates the two arms is only how the journeys themselves are counted. Keeping
// only the neighbors that lie exactly on a shortest path makes the graph a DAG by
// construction (Dist strictly decreases across every such road), which is what lets
// the composed arm use DagFold's memoized fold instead of CheckedFold's runtime
// cycle defense.
internal static class NumberOfWaysToArriveAtDestinationSolution
{
    // The textbook answer: recurse into every neighbor exactly one shortest-path
    // step closer and add up what comes back, with no memory between branches, so an
    // intersection reachable by k distinct prefixes is recounted k times - genuinely
    // exponential wherever the same intersection is reached two ways at every layer.
    // Plain recursion over the node's own adjacency list; it is the arm the fold
    // below has to justify itself against.
    public static long CountWaysByNaiveDfs(int intersectionCount, int[][] roads)
    {
        var graph = WaysGraph.Build(intersectionCount, roads);

        return CountWaysByNaiveDfs(graph);
    }

    public static long CountWaysByNaiveDfs(WaysGraph graph) => CountWaysFrom(graph.Start);

    private static long CountWaysFrom(WaysNode node)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var (weight, target) in node.Edges)
        {
            if (node.Dist - weight == target.Dist)
            {
                total = (total + CountWaysFrom(target)) % ModularArithmetic.Modulo;
            }
        }

        return total;
    }

    // This repo's own memoized fold: WaysChildTopology turns "step only onto a road
    // that stays on a shortest path" into an IDagTopology, and DagFold.Fold then
    // visits each distinct intersection once and reuses its count everywhere it is
    // reached from - one Combine per intersection instead of one per journey prefix.
    public static long CountWaysByDagFold(int intersectionCount, int[][] roads)
    {
        var graph = WaysGraph.Build(intersectionCount, roads);

        return CountWaysByDagFold(graph);
    }

    public static long CountWaysByDagFold(WaysGraph graph) =>
        DagFold.Fold<
            WaysNode, WaysChildTopology, ListChildren<WaysNode>,
            NaturalChildOrder<WaysNode, ListChildren<WaysNode>>, ListChildren<WaysNode>,
            WaysCountAlgebra, long>(graph.Start);
}
