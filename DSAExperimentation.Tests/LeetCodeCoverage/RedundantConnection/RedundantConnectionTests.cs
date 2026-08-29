using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RedundantConnection;

// LeetCode 684. Redundant Connection: given a tree with one extra edge added, find
// the extra edge - the first edge that connects two nodes already in the same
// DisjointSet component.
public sealed partial class RedundantConnectionTests
{
    [Fact]
    public void FindRedundantEdge_ClassicExample_ReturnsTheCycleClosingEdge()
    {
        (int First, int Second)[] edges = [(1, 2), (1, 3), (2, 3)];

        var redundant = FindRedundantEdge(edges, nodeCount: 3);

        Assert.Equal((2, 3), redundant);
    }

    // Precondition (guaranteed by LeetCode 684's own constraints): edges describes a
    // tree plus exactly one extra edge, so the loop below always returns before
    // falling through.
    private static (int First, int Second) FindRedundantEdge((int First, int Second)[] edges, int nodeCount)
    {
        var components = new DisjointSet(nodeCount + 1);

        foreach (var (first, second) in edges)
        {
            if (components.IsConnected(first, second))
            {
                return (first, second);
            }

            components.Union(first, second);
        }

        return default;
    }
}
