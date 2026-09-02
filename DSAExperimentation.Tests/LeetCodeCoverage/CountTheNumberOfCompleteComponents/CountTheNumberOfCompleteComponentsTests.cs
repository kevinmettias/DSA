using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfCompleteComponents;

// LeetCode 2685. Count the Number of Complete Components: a component is complete
// when every pair of its nodes is directly connected - equivalent to its edge count
// matching n*(n-1)/2 for its own node count n. DisjointSet groups nodes into
// components; a per-root tally of node/edge counts is all that's left to check.
public sealed partial class CountTheNumberOfCompleteComponentsTests
{
    [Fact]
    public void CountCompleteComponents_TwoCompleteComponentsAndAnIsolatedNode_ReturnsThree()
    {
        int[][] edges = [[0, 1], [0, 2], [1, 2], [3, 4]];

        Assert.Equal(3, CountCompleteComponents(n: 6, edges));
    }

    [Fact]
    public void CountCompleteComponents_OneComponentMissingAnEdge_ExcludesItFromTheCount()
    {
        int[][] edges = [[0, 1], [0, 2], [1, 2], [3, 4], [3, 5]];

        Assert.Equal(1, CountCompleteComponents(n: 6, edges));
    }

    private static int CountCompleteComponents(int n, int[][] edges)
    {
        var components = new DisjointSet(n);
        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var nodeCountByRoot = new Dictionary<int, int>();
        var edgeCountByRoot = new Dictionary<int, int>();

        for (var node = 0; node < n; node++)
        {
            var root = components.Find(node);
            nodeCountByRoot[root] = nodeCountByRoot.GetValueOrDefault(root) + 1;
        }

        foreach (var edge in edges)
        {
            var root = components.Find(edge[0]);
            edgeCountByRoot[root] = edgeCountByRoot.GetValueOrDefault(root) + 1;
        }

        var complete = 0;
        foreach (var (root, nodeCount) in nodeCountByRoot)
        {
            var edgeCount = edgeCountByRoot.GetValueOrDefault(root);
            if (edgeCount == nodeCount * (nodeCount - 1) / 2)
            {
                complete++;
            }
        }

        return complete;
    }
}
