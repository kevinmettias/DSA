using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.JumpGameV.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameV;

// LeetCode 1340. Jump Game V: from index i you may jump to index j (|i-j| <= d) only
// when arr[i] is strictly greater than every value strictly between i and j *and*
// than arr[j] itself - every edge in that reachability relation therefore drops in
// value, making it a DAG by construction. Modeled as an implicit graph and solved
// with this repo's own TopologicalSort.TrySort (Kahn's algorithm) to get a safe
// processing order, then one linear longest-path relaxation pass over that order -
// not the textbook per-start memoized DFS.
public sealed partial class JumpGameVTests
{
    [Theory]
    [InlineData(new[] { 6, 4, 14, 6, 8, 13, 9, 7, 10, 6, 12 }, 2, 4)]
    [InlineData(new[] { 3, 3, 3, 3, 3 }, 3, 1)]
    [InlineData(new[] { 1 }, 1, 1)]
    public void MaxIndicesVisited_TopologicalOrderLongestPath_ReturnsExpectedCount(
        int[] arr, int d, int expected)
        => Assert.Equal(expected, MaxIndicesVisited(arr, d));

    private static int MaxIndicesVisited(int[] arr, int d)
    {
        var nodes = BuildReachabilityDag(arr, d);

        var isDag = TopologicalSort.TrySort<
            JumpNode, JumpTopology, ListChildren<JumpNode>,
            NaturalChildOrder<JumpNode, ListChildren<JumpNode>>, ListChildren<JumpNode>>(
            nodes, out var ordering);
        Assert.True(isDag);

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

        return longestPath.Values.Max();
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
}
