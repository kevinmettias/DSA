using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// LeetCode 1514. Path with Maximum Probability: the greatest product of edge success
// probabilities along any path between two vertices of an undirected graph, or 0 when
// no path exists.
//
// Maximizing a product of probabilities is the same problem as minimizing the sum of
// each edge's -log(probability) - a non-negative transform (a probability in (0, 1]
// has log <= 0) that turns this into exactly ShortestPath.Dijkstra's own precondition
// (non-negative edge weights, minimize distance), with zero changes to Dijkstra
// itself. The answer is Math.Exp(-distance): undoing the log transform.
//
// The exhaustive walk is the textbook arm that reduction has to justify itself
// against, and it lives here rather than in the benchmark so the same examples assert
// both.
internal static class PathWithMaximumProbabilitySolution
{
    // LC 1514 reports "no path from start to end" as probability 0, not as
    // LeetCodeAnswer.None - the answer is a probability, and 0 is a real one.
    private const double UnreachableProbability = 0.0;

    private const double CertainProbability = 1.0;

    // The textbook answer: walk every simple path from start to end and keep the best
    // running product. Exponential in the vertex count - every incident edge branches
    // the path count - and written with nothing but the call stack and a bool[] guard
    // marking the vertices already on the current path, which is what keeps a cycle in
    // the undirected input from looping forever.
    public static double MaxProbabilityByExhaustiveDfs(
        int nodeCount, int[][] edges, double[] successProbabilities, (int Start, int End) endpoints)
    {
        var graph = ProbabilityGraph.Build(nodeCount, edges, successProbabilities);

        return MaxProbabilityByExhaustiveDfs(graph, endpoints.Start, endpoints.End);
    }

    public static double MaxProbabilityByExhaustiveDfs(ProbabilityGraph graph, int start, int end)
    {
        var walk = new PathWalk(graph, end, new bool[graph.NodeCount]);

        return BestProduct(walk, start, CertainProbability);
    }

    private static double BestProduct(PathWalk walk, int node, double productSoFar)
    {
        var best = node == walk.End ? productSoFar : UnreachableProbability;
        walk.OnPath[node] = true;

        foreach (var (probability, next) in walk.Graph.Adjacency[node])
        {
            if (!walk.OnPath[next])
            {
                var throughNext = BestProduct(walk, next, productSoFar * probability);
                best = Math.Max(best, throughNext);
            }
        }

        walk.OnPath[node] = false;

        return best;
    }

    // The parts of the exhaustive walk that never change between recursive calls,
    // carried as one value the way OpenTheLockSolution's own TurnWalk is.
    private readonly record struct PathWalk(ProbabilityGraph Graph, int End, bool[] OnPath);

    // This repo's own Dijkstra over the -log(probability) reweighting: one shortest-
    // path run from start settles every vertex, and the answer is that distance
    // transformed back into a product.
    public static double MaxProbabilityByDijkstra(
        int nodeCount, int[][] edges, double[] successProbabilities, (int Start, int End) endpoints)
    {
        var graph = ProbabilityGraph.Build(nodeCount, edges, successProbabilities);

        return MaxProbabilityByDijkstra(graph, endpoints.Start, endpoints.End);
    }

    public static double MaxProbabilityByDijkstra(ProbabilityGraph graph, int start, int end)
    {
        var distances = ShortestPath.Dijkstra<
            ProbabilityNode, ProbabilityTopology, ListEdges<ProbabilityNode, double>, double>(graph.Nodes[start]);

        return distances.TryGetValue(graph.Nodes[end], out var cost)
            ? Math.Exp(-cost)
            : UnreachableProbability;
    }
}
