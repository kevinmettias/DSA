using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountUnreachablePairsOfNodesInAnUndirectedGraph;

// LeetCode 2316. Count Unreachable Pairs of Nodes in an Undirected Graph: union
// every edge into this repo's own DisjointSet (NumberOfProvincesTests/
// NumberOfOperationsToMakeNetworkConnectedTests precedent), then tally each
// component's size via HashMap<root,size> (LargestComponentSizeByCommonFactorTests'
// own TallyComponent shape, read back afterward through HashMap.Values so each
// distinct component contributes exactly once). Every pair of nodes is either
// inside the same component (reachable) or split across two different components
// (unreachable), so the answer is total pairs C(n,2) minus the reachable pairs
// summed component-by-component - no separate reachability search needed once
// component sizes are known.
public sealed partial class CountUnreachablePairsOfNodesInAnUndirectedGraphTests
{
    [Fact]
    public void CountPairs_TwoComponents_ReturnsPairsSplitAcrossThem()
    {
        // {0,1,2} (size 3) and {3,4,5,6} (size 4); verified independently against a
        // brute-force pairwise BFS reachability check.
        int[][] edges = [[0, 2], [0, 1], [1, 2], [3, 4], [4, 5], [5, 6]];

        var unreachable = CountPairs(7, edges);

        Assert.Equal(12, unreachable);
    }

    [Fact]
    public void CountPairs_NoEdges_EveryPairIsUnreachable()
    {
        var unreachable = CountPairs(4, []);

        Assert.Equal(6, unreachable);
    }

    [Fact]
    public void CountPairs_FullyConnectedTriangle_ReturnsZero()
    {
        int[][] edges = [[0, 1], [1, 2], [2, 0]];

        var unreachable = CountPairs(3, edges);

        Assert.Equal(0, unreachable);
    }

    private static long CountPairs(int n, int[][] edges)
    {
        var components = new DisjointSet(n);
        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var sizeByRoot = new HashMap<int, long>();
        for (var i = 0; i < n; i++)
        {
            var root = components.Find(i);
            sizeByRoot.TryGetValue(root, out var size);
            sizeByRoot.Set(root, size + 1);
        }

        var totalPairs = (long)n * (n - 1) / 2;
        var reachablePairs = 0L;

        foreach (var size in sizeByRoot.Values)
        {
            reachablePairs += size * (size - 1) / 2;
        }

        return totalPairs - reachablePairs;
    }
}
