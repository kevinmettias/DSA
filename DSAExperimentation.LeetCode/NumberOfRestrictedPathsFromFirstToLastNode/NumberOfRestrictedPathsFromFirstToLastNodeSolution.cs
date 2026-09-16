using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// LeetCode 1786. Number of Restricted Paths From First to Last Node: an undirected
// weighted graph on nodes 1..n, where distanceToLastNode(x) is x's shortest distance
// to node n. A path from node 1 to node n is restricted when that distance strictly
// decreases at every step. Count them, modulo 1e9+7.
//
// Both strategies share RestrictedPathGraph's single Dijkstra run - the distances
// are what the "restricted" rule is stated against, not part of counting - so what
// separates them is only how the walk itself is counted. The restriction makes the
// graph a DAG by construction (Dist strictly decreases, so no walk can revisit a
// node), which is what lets the composed arm use DagFold's memoized fold instead of
// CheckedFold's runtime cycle defense.
internal static class NumberOfRestrictedPathsFromFirstToLastNodeSolution
{
    // The textbook answer: recurse into every strictly-closer neighbor and add up
    // what comes back, with no memory between branches, so a node reachable by k
    // distinct prefixes is recounted k times - genuinely exponential on a graph
    // where the same node is reached two ways at every layer. Plain recursion over
    // the node's own adjacency list; it is the arm the fold below has to justify
    // itself against.
    public static long CountRestrictedPathsByNaiveDfs(int nodeCount, int[][] edges)
    {
        var graph = RestrictedPathGraph.Build(nodeCount, edges);

        return CountRestrictedPathsByNaiveDfs(graph);
    }

    public static long CountRestrictedPathsByNaiveDfs(RestrictedPathGraph graph) =>
        CountPathsFrom(graph.First);

    private static long CountPathsFrom(RestrictedPathNode node)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var (_, target) in node.Edges)
        {
            if (target.Dist < node.Dist)
            {
                total = (total + CountPathsFrom(target)) % ModularArithmetic.Modulo;
            }
        }

        return total;
    }

    // This repo's own memoized fold: RestrictedPathChildTopology turns "step only to
    // a strictly-closer neighbor" into an IDagTopology, and DagFold.Fold then visits
    // each distinct node once and reuses its count everywhere it is reached from -
    // one Combine per node instead of one per path prefix.
    public static long CountRestrictedPathsByDagFold(int nodeCount, int[][] edges)
    {
        var graph = RestrictedPathGraph.Build(nodeCount, edges);

        return CountRestrictedPathsByDagFold(graph);
    }

    public static long CountRestrictedPathsByDagFold(RestrictedPathGraph graph) =>
        DagFold.Fold<
            RestrictedPathNode, RestrictedPathChildTopology, ListChildren<RestrictedPathNode>,
            NaturalChildOrder<RestrictedPathNode, ListChildren<RestrictedPathNode>>, ListChildren<RestrictedPathNode>,
            RestrictedPathCountAlgebra, long>(graph.First);
}
