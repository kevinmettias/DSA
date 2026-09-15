using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

// LeetCode 3666. Minimum Operations to Equalize Binary String: each operation
// flips exactly k of the string's indices; find the fewest operations to reach all
// '1's, or -1 if that is unreachable.
//
// Only the current zero-count matters, not which indices are zero - so the puzzle
// is a shortest-path query on EqualizeStateGraph's zero-count graph (see its own
// doc comment for the reachable-range derivation), reduced to a lookup exactly the
// way OpenTheLockSolution reduces LC 752 to one on Domain.Locks' wheel-turn graph.
internal static class MinimumOperationsToEqualizeBinaryStringSolution
{
    // The textbook answer: BCL Queue + a visited array, generating each reachable
    // zero-count on the fly via EqualizeStateGraph's pure range arithmetic and
    // never materializing the graph - the arm the composed solution below has to
    // justify itself against.
    public static int MinOperationsByMutationQueue(string s, int k)
    {
        var n = s.Length;
        var zeroCount = CountZeros(s);

        if (zeroCount == 0)
        {
            return 0;
        }

        var visited = new bool[n + 1];
        var queue = new Queue<(int ZeroCount, int Ops)>();
        visited[zeroCount] = true;
        queue.Enqueue((zeroCount, 0));

        return ShortestPathLength(queue, visited, n, k);
    }

    // The BFS itself: expand the frontier one flip at a time until the all-ones state
    // surfaces, or report unreachable once the frontier empties.
    private static int ShortestPathLength(
        Queue<(int ZeroCount, int Ops)> queue, bool[] visited, int n, int k)
    {
        while (queue.Count > 0)
        {
            var (currentZeroCount, ops) = queue.Dequeue();

            if (currentZeroCount == 0)
            {
                return ops;
            }

            foreach (var reachable in EqualizeStateGraph.ReachableZeroCounts(currentZeroCount, n, k))
            {
                if (!visited[reachable])
                {
                    visited[reachable] = true;
                    queue.Enqueue((reachable, ops + 1));
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    private static int CountZeros(string s)
    {
        var zeroCount = 0;

        foreach (var c in s)
        {
            if (c == '0')
            {
                zeroCount++;
            }
        }

        return zeroCount;
    }

    // This repo's own BFS: Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already exactly "distance from a root to every
    // node", so the puzzle reduces to one lookup in the result - the same
    // composition OpenTheLockSolution uses for LC 752.
    public static int MinOperationsByReduceGraph(string s, int k)
    {
        var n = s.Length;
        var zeroCount = CountZeros(s);
        var graph = EqualizeStateGraph.Build(n, k);

        return MinOperationsByReduceGraph(graph, zeroCount);
    }

    public static int MinOperationsByReduceGraph(EqualizeStateGraph graph, int zeroCount)
    {
        if (zeroCount == 0)
        {
            return 0;
        }

        var startNode = graph.Node(zeroCount);

        var distances = Reduce.Graph<
            EqualizeStateNode, EqualizeStateTopology, ListChildren<EqualizeStateNode>,
            NaturalChildOrder<EqualizeStateNode, ListChildren<EqualizeStateNode>>, ListChildren<EqualizeStateNode>,
            BreadthFirstReduceOrder<EqualizeStateNode>,
            DistanceMapReduceAlgebra<EqualizeStateNode>, Dictionary<EqualizeStateNode, int>>(startNode);

        var targetNode = graph.Node(0);

        return distances.TryGetValue(targetNode, out var distance) ? distance : LeetCodeAnswer.None;
    }
}
