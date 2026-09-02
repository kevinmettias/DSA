using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachableNodesWithRestrictions;

// LeetCode 2368. Reachable Nodes With Restrictions: the input tree stays a tree
// even after restricted nodes are dropped from it - removing a node's edges only
// ever prunes whole subtrees, never reconnects two otherwise-disjoint pieces - so
// unioning every edge whose endpoints are BOTH unrestricted into this repo's own
// DisjointSet (CountUnreachablePairsOfNodesInAnUndirectedGraphTests' own
// union-every-edge shape) and counting how many unrestricted nodes land in node
// 0's component answers the question directly, with no separate BFS/DFS needed.
public sealed partial class ReachableNodesWithRestrictionsTests
{
    [Fact]
    public void ReachableNodes_ClassicExample_CountsNodesBeforeTheRestrictedBranch()
    {
        int[][] edges = [[0, 1], [1, 2], [3, 1], [4, 0], [0, 5], [5, 6]];
        int[] restricted = [4, 5];

        var reachable = ReachableNodes(7, edges, restricted);

        Assert.Equal(4, reachable);
    }

    [Fact]
    public void ReachableNodes_RestrictedNodesCutOffMostOfTheTree_CountsOnlyTheSurvivingBranch()
    {
        int[][] edges = [[0, 1], [0, 2], [0, 5], [0, 4], [3, 2], [6, 5]];
        int[] restricted = [4, 2, 1];

        var reachable = ReachableNodes(7, edges, restricted);

        Assert.Equal(3, reachable);
    }

    [Fact]
    public void ReachableNodes_NoRestrictedNodes_EveryNodeIsReachable()
    {
        int[][] edges = [[0, 1], [1, 2], [2, 3]];

        var reachable = ReachableNodes(4, edges, []);

        Assert.Equal(4, reachable);
    }

    private static int ReachableNodes(int n, int[][] edges, int[] restricted)
    {
        var isRestricted = new bool[n];
        foreach (var node in restricted)
        {
            isRestricted[node] = true;
        }

        var components = new DisjointSet(n);
        foreach (var edge in edges)
        {
            if (!isRestricted[edge[0]] && !isRestricted[edge[1]])
            {
                components.Union(edge[0], edge[1]);
            }
        }

        var reachable = 0;
        for (var i = 0; i < n; i++)
        {
            if (!isRestricted[i] && components.IsConnected(i, 0))
            {
                reachable++;
            }
        }

        return reachable;
    }
}
