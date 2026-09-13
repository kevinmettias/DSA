using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

// LeetCode 847. Shortest Path Visiting All Nodes: the length of the shortest walk
// that touches every node of a connected undirected graph, starting anywhere and
// revisiting nodes and edges freely.
//
// Both strategies search the same space - (current node, bitmask of nodes visited
// so far) states, where an input edge moves you to the neighbor with that
// neighbor's bit set - and both look for the first state whose mask is full. They
// differ only in how that search is run.
internal static class ShortestPathVisitingAllNodesSolution
{
    // The textbook answer: true multi-source BFS over a BCL Queue, with every
    // node's own single-bit start state seeded at depth 0 at once and a BCL
    // HashSet for the visited set. Deliberately written without this repo's
    // primitives - it is the arm the composed strategy below has to justify
    // itself against, and it shares one frontier across all n starts, which is
    // exactly the advantage the composed arm gives up.
    public static int ShortestPathLengthByMutationQueue(int[][] graph)
    {
        var fullMask = (1 << graph.Length) - 1;
        var visited = new HashSet<(int Node, int Mask)>();
        var queue = new Queue<(int Node, int Mask, int Steps)>();

        SeedStartStates(graph.Length, queue, visited);

        return RunMutationQueueBfs(graph, queue, visited, fullMask);
    }

    private static void SeedStartStates(
        int nodeCount,
        Queue<(int Node, int Mask, int Steps)> queue,
        HashSet<(int Node, int Mask)> visited)
    {
        for (var start = 0; start < nodeCount; start++)
        {
            if (visited.Add((start, 1 << start)))
            {
                queue.Enqueue((start, 1 << start, 0));
            }
        }
    }

    private static int RunMutationQueueBfs(
        int[][] graph,
        Queue<(int Node, int Mask, int Steps)> queue,
        HashSet<(int Node, int Mask)> visited,
        int fullMask)
    {
        while (queue.Count > 0)
        {
            var (node, mask, steps) = queue.Dequeue();

            if (mask == fullMask)
            {
                return steps;
            }

            EnqueueNeighborStates(graph, node, mask, steps, queue, visited);
        }

        return LeetCodeAnswer.None;
    }

    private static void EnqueueNeighborStates(
        int[][] graph,
        int node,
        int mask,
        int steps,
        Queue<(int Node, int Mask, int Steps)> queue,
        HashSet<(int Node, int Mask)> visited)
    {
        foreach (var neighbor in graph[node])
        {
            var nextMask = mask | (1 << neighbor);

            if (visited.Add((neighbor, nextMask)))
            {
                queue.Enqueue((neighbor, nextMask, steps + 1));
            }
        }
    }

    // This repo's own BFS: Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already "distance from a root to every node",
    // so one call per possible start gives every full-mask state's depth and the
    // answer is the minimum over all of them. Min-of-mins commutes with the
    // multi-source frontier above because every edge here costs 1 - the same
    // reasoning DistanceMapReduceAlgebra's own doc comment relies on - so this
    // arm computes the same number while doing strictly more total work, which
    // is the honest cost the benchmark measures.
    public static int ShortestPathLengthByReduceGraph(int[][] graph) =>
        ShortestPathLengthByReduceGraph(VisitStateGraph.Build(graph));

    public static int ShortestPathLengthByReduceGraph(VisitStateGraph stateGraph)
    {
        var shortest = int.MaxValue;

        foreach (var startNode in stateGraph.StartNodes)
        {
            var fromStart = ShortestPathFromStart(startNode, stateGraph.FullMask);

            if (fromStart < shortest)
            {
                shortest = fromStart;
            }
        }

        return shortest == int.MaxValue ? LeetCodeAnswer.None : shortest;
    }

    // Single-source BFS distance map from one start state, reduced to the first
    // depth at which any state has visited every node.
    private static int ShortestPathFromStart(VisitStateNode startNode, int fullMask)
    {
        var distances = Reduce.Graph<
            VisitStateNode, VisitStateTopology, ListChildren<VisitStateNode>,
            NaturalChildOrder<VisitStateNode, ListChildren<VisitStateNode>>, ListChildren<VisitStateNode>,
            BreadthFirstReduceOrder<VisitStateNode>,
            DistanceMapReduceAlgebra<VisitStateNode>, Dictionary<VisitStateNode, int>>(startNode);

        var shortest = int.MaxValue;

        foreach (var (state, distance) in distances)
        {
            if (state.Mask == fullMask && distance < shortest)
            {
                shortest = distance;
            }
        }

        return shortest;
    }
}
