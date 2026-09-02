using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.LargestColorValueInADirectedGraph.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestColorValueInADirectedGraph;

// LeetCode 1857. Largest Color Value in a Directed Graph: TopologicalSort.TrySort
// both detects the cycle case (returns false -> answer -1, the same
// leftover-in-degree signal Course Schedule already relies on) and hands
// back a dependency-respecting order to run a per-color counting DP over -
// count[node][c] is the most c-colored nodes on any path ending at node,
// relaxed onto each successor only once its predecessor's own count is
// fully known, which Kahn's order guarantees.
public sealed partial class LargestColorValueInADirectedGraphTests
{
    private const int AlphabetSize = 26;

    [Fact]
    public void LargestPathValue_LeetCodeExampleOne_ReturnsThree()
    {
        var nodes = BuildGraph("abaca", [[0, 1], [0, 2], [2, 3], [3, 4]]);

        Assert.Equal(3, LargestPathValue(nodes));
    }

    [Fact]
    public void LargestPathValue_LeetCodeExampleTwo_ReturnsNegativeOneOnCycle()
    {
        var nodes = BuildGraph("a", [[0, 0]]);

        Assert.Equal(-1, LargestPathValue(nodes));
    }

    private static List<ColorGraphNode> BuildGraph(string colors, int[][] edges)
    {
        var nodes = colors.Select((color, id) => new ColorGraphNode(id, color - 'a')).ToList();

        foreach (var edge in edges)
        {
            nodes[edge[0]].Successors.Add(nodes[edge[1]]);
        }

        return nodes;
    }

    private static int LargestPathValue(List<ColorGraphNode> nodes)
    {
        var sorted = TopologicalSort.TrySort<
            ColorGraphNode, ColorGraphTopology, ListChildren<ColorGraphNode>,
            NaturalChildOrder<ColorGraphNode, ListChildren<ColorGraphNode>>, ListChildren<ColorGraphNode>>(
            nodes, out var ordering);

        if (!sorted)
        {
            return -1;
        }

        var counts = nodes.ToDictionary(node => node, _ => new int[AlphabetSize]);
        var best = 0;

        foreach (var node in ordering)
        {
            var nodeBest = RelaxNode(node, counts);
            best = Math.Max(best, nodeBest);
        }

        return best;
    }

    // Increments node's own color count and relaxes it forward onto every
    // successor (each child's count[c] becomes the max of its own and node's,
    // since Kahn's order guarantees node is fully finalized before any child
    // is visited). Returns node's own updated count for its color.
    private static int RelaxNode(ColorGraphNode node, Dictionary<ColorGraphNode, int[]> counts)
    {
        var nodeCounts = counts[node];
        nodeCounts[node.Color]++;

        var children = ColorGraphTopology.GetChildren(node);
        for (var i = 0; i < children.Count; i++)
        {
            var childCounts = counts[children.Get(i)];
            for (var c = 0; c < AlphabetSize; c++)
            {
                childCounts[c] = Math.Max(childCounts[c], nodeCounts[c]);
            }
        }

        return nodeCounts[node.Color];
    }
}
