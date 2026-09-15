using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

// LeetCode 3419. Minimize the Maximum Edge Weight of Graph: remove edges so every
// node can still reach node 0, minimizing the largest weight left behind.
//
// `threshold` is part of LeetCode's signature but never consulted: reaching node 0
// only ever needs ONE outgoing edge per node - the step toward node 0 on its own
// path - so any threshold >= 1 (the problem's own lower bound) already clears it.
// What actually decides feasibility for a candidate max weight W is a pure
// reachability question: with only edges of weight <= W kept, can every node still
// reach 0? That is monotonic in W (a higher W only keeps more edges), so both
// strategies binary search on W over the same EdgeWeightGraph and differ only in
// how "is W feasible" gets answered - a plain BFS or a Reduce.Graph walk.
internal static class MinimizeTheMaximumEdgeWeightOfGraphSolution
{
    // The textbook approach: a BCL Queue<int>/bool[] BFS over the graph's own
    // reversed adjacency. Only the input container (EdgeWeightGraph) is this
    // repo's; the search itself is deliberately plain, the arm the composed
    // strategy has to beat.
    public static int MinMaxWeightByBinarySearchBfs(int n, int[][] edges, int threshold)
    {
        var graph = EdgeWeightGraph.Build(n, edges);

        return MinMaxWeightByBinarySearchBfs(graph, threshold);
    }

    public static int MinMaxWeightByBinarySearchBfs(EdgeWeightGraph graph, int threshold) =>
        BinarySearchWeight(graph.MaxWeight, new BfsFeasibility(graph));

    private static bool IsFeasibleByBfs(EdgeWeightGraph graph, int maxWeight)
    {
        var visited = new bool[graph.NodeCount];
        visited[0] = true;
        var reachedCount = 1;
        var queue = new Queue<int>();
        queue.Enqueue(0);

        while (queue.Count > 0)
        {
            foreach (var (to, weight) in graph.NeighborsOf(queue.Dequeue()))
            {
                if (weight <= maxWeight && !visited[to])
                {
                    visited[to] = true;
                    reachedCount++;
                    queue.Enqueue(to);
                }
            }
        }

        return reachedCount == graph.NodeCount;
    }

    // Composed: the same binary search, but feasibility is answered by
    // Reduce.Graph over EdgeWeightTopology - the reversed, weight-filtered
    // adjacency this problem alone needs - the same way GridShortestPath answers
    // "how far" over GridTopology.
    public static int MinMaxWeightByReduceGraphBinarySearch(int n, int[][] edges, int threshold)
    {
        var graph = EdgeWeightGraph.Build(n, edges);

        return MinMaxWeightByReduceGraphBinarySearch(graph, threshold);
    }

    public static int MinMaxWeightByReduceGraphBinarySearch(EdgeWeightGraph graph, int threshold) =>
        BinarySearchWeight(graph.MaxWeight, new ReduceGraphFeasibility(graph));

    private static bool IsFeasibleByReduceGraph(EdgeWeightGraph graph, int maxWeight)
    {
        var root = new EdgeWeightNode(0, graph, maxWeight);

        var distances = Reduce.Graph<
            EdgeWeightNode, EdgeWeightTopology, EdgeWeightChildren,
            NaturalChildOrder<EdgeWeightNode, EdgeWeightChildren>, EdgeWeightChildren,
            BreadthFirstReduceOrder<EdgeWeightNode>,
            DistanceMapReduceAlgebra<EdgeWeightNode>, Dictionary<EdgeWeightNode, int>>(root);

        return distances.Count == graph.NodeCount;
    }

    // The one question the two strategies answer with different code: with only edges
    // of weight <= `maxWeight` kept, can every node still reach node 0? The graph is
    // settled before either strategy asks, so only the candidate weight varies from
    // probe to probe.
    private interface IWeightFeasibility
    {
        bool HoldsFor(int maxWeight);
    }

    // Smallest feasible weight in [1, maxWeight], or LeetCodeAnswer.None if even
    // maxWeight (every edge kept) still can't reach every node.
    private static int BinarySearchWeight(int maxWeight, IWeightFeasibility isFeasible)
    {
        if (!isFeasible.HoldsFor(maxWeight))
        {
            return LeetCodeAnswer.None;
        }

        return SmallestFeasibleWeight(maxWeight, isFeasible);
    }

    // The search itself: the whole range is known feasible and a weight of 0 is
    // known not to be (it keeps no edges at all), so the invariant narrows until only
    // the smallest feasible weight is left.
    private static int SmallestFeasibleWeight(int maxWeight, IWeightFeasibility isFeasible)
    {
        var low = 1;
        var high = maxWeight;

        while (low < high)
        {
            var mid = low + (high - low) / 2;

            if (isFeasible.HoldsFor(mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    // The baseline arm's mechanism: a BCL Queue<int>/bool[] BFS over the graph's own
    // reversed adjacency.
    private sealed class BfsFeasibility(EdgeWeightGraph graph) : IWeightFeasibility
    {
        public bool HoldsFor(int maxWeight) => IsFeasibleByBfs(graph, maxWeight);
    }

    // The composed arm's mechanism: Reduce.Graph over EdgeWeightTopology - the
    // reversed, weight-filtered adjacency this problem alone needs.
    private sealed class ReduceGraphFeasibility(EdgeWeightGraph graph) : IWeightFeasibility
    {
        public bool HoldsFor(int maxWeight) => IsFeasibleByReduceGraph(graph, maxWeight);
    }
}
