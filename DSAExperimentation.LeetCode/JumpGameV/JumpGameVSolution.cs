using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.JumpGameV;

// LeetCode 1340. Jump Game V: from index i you may jump to index j (|i-j| <= d)
// only when arr[i] is strictly greater than every value strictly between i and j
// *and* than arr[j] itself. Report the most indices any single run of jumps can
// visit, counting the starting index.
//
// Every edge in that reachability relation drops in value, so the relation is a
// DAG by construction and the answer is its longest path (in nodes). The two
// strategies below reach that same answer from opposite directions: the textbook
// memoized DFS rooted at each start, and one topological pass over the whole
// graph.
internal static class JumpGameVSolution
{
    // The textbook answer: a BCL Dictionary<int, int> memo and one recursive
    // call tree rooted at each of the n starting indices, never materializing
    // the graph at all. Deliberately written without this repo's primitives -
    // it is the arm the composed strategy below has to justify itself against.
    public static int MaxIndicesVisitedByMemoizedDfs(int[] arr, int d)
    {
        var memo = new Dictionary<int, int>();
        var best = 0;

        for (var start = 0; start < arr.Length; start++)
        {
            best = Math.Max(best, LongestPathFrom(arr, d, start, memo));
        }

        return best;
    }

    // This repo's own composition: build the reachability relation once as an
    // explicit graph, hand it to TopologicalSort.TrySort (Kahn's algorithm) for
    // a safe processing order, then relax longest-path lengths forward in a
    // single linear pass over that order - no recursion, and every node's value
    // final the first time it is read.
    public static int MaxIndicesVisitedByTopologicalSort(int[] arr, int d)
    {
        var nodes = BuildReachabilityDag(arr, d);

        // Every edge drops in value, so the graph is acyclic by construction and
        // TrySort always succeeds here; the discard records that rather than
        // inventing an unreachable failure branch.
        _ = TopologicalSort.TrySort<
            JumpNode, JumpTopology, ListChildren<JumpNode>,
            NaturalChildOrder<JumpNode, ListChildren<JumpNode>>, ListChildren<JumpNode>>(
            nodes, out var ordering);

        return ComputeLongestPaths(ordering).Values.Max();
    }

    private static JumpNode[] BuildReachabilityDag(int[] arr, int d)
    {
        var nodes = new JumpNode[arr.Length];

        for (var i = 0; i < arr.Length; i++)
        {
            nodes[i] = new JumpNode(i);
        }

        for (var i = 0; i < arr.Length; i++)
        {
            for (var j = i + 1; j <= Math.Min(arr.Length - 1, i + d) && arr[j] < arr[i]; j++)
            {
                nodes[i].ReachableIndices.Add(nodes[j]);
            }

            for (var j = i - 1; j >= Math.Max(0, i - d) && arr[j] < arr[i]; j--)
            {
                nodes[i].ReachableIndices.Add(nodes[j]);
            }
        }

        return nodes;
    }

    private static Dictionary<JumpNode, int> ComputeLongestPaths(IEnumerable<JumpNode> ordering)
    {
        var longestPath = new Dictionary<JumpNode, int>();

        foreach (var node in ordering)
        {
            longestPath.TryAdd(node, 1);

            foreach (var next in node.ReachableIndices)
            {
                var candidate = longestPath[node] + 1;

                if (candidate > longestPath.GetValueOrDefault(next, 1))
                {
                    longestPath[next] = candidate;
                }
            }
        }

        return longestPath;
    }

    // MaxIndicesVisitedByMemoizedDfs' helper, placed last because it recurses:
    // a helper with more than one call site is ordered after every
    // single-caller helper's own subtree, not beside the method it serves.
    private static int LongestPathFrom(int[] arr, int d, int i, Dictionary<int, int> memo)
    {
        if (memo.TryGetValue(i, out var cached))
        {
            return cached;
        }

        var best = 1;

        for (var j = i + 1; j <= Math.Min(arr.Length - 1, i + d) && arr[j] < arr[i]; j++)
        {
            best = Math.Max(best, 1 + LongestPathFrom(arr, d, j, memo));
        }

        for (var j = i - 1; j >= Math.Max(0, i - d) && arr[j] < arr[i]; j--)
        {
            best = Math.Max(best, 1 + LongestPathFrom(arr, d, j, memo));
        }

        memo[i] = best;
        return best;
    }
}
