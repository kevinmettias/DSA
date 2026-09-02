using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.ShortestDistanceToTargetStringInACircularArray.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestDistanceToTargetStringInACircularArray;

// LeetCode 2515. Shortest Distance to Target String in a Circular Array: each index
// is a node with exactly two edges - one step left, one step right, wrapping mod
// words.Length - so the shortest distance from startIndex to any index equal to
// target is exactly Reduce.Graph's own BFS distance map (DistanceMapReduceAlgebra),
// minimized over every node whose word matches target. Same Reduce.Graph +
// DistanceMapReduceAlgebra composition SmallestIntegerDivisibleByKTests already
// uses, just over a 2-neighbor circular graph instead of a single-successor one -
// and this repo's own graph-tier BFS (TrackedVisitGuard) needs no bespoke
// wraparound-distance formula, since exploring both directions simultaneously is
// exactly what BFS already does.
public sealed partial class ShortestDistanceToTargetStringInACircularArrayTests
{
    [Theory]
    [InlineData(new[] { "hello", "i", "am", "leetcode", "hello" }, "hello", 1, 1)]
    [InlineData(new[] { "i", "am", "leetcode", "leetcode" }, "leetcode", 0, 1)]
    [InlineData(new[] { "i", "eat", "leetcode" }, "ate", 0, -1)]
    public void ClosestTarget_LeetCodeExamples_ReturnsShortestCircularDistanceOrNegativeOne(
        string[] words, string target, int startIndex, int expected)
    {
        var actual = ClosestTarget(words, target, startIndex);
        Assert.Equal(expected, actual);
    }

    private static int ClosestTarget(string[] words, string target, int startIndex)
    {
        var nodes = BuildCircularGraph(words);
        var distances = Reduce.Graph<
            CircularArrayNode, CircularArrayTopology, ListChildren<CircularArrayNode>,
            NaturalChildOrder<CircularArrayNode, ListChildren<CircularArrayNode>>, ListChildren<CircularArrayNode>,
            BreadthFirstReduceOrder<CircularArrayNode>,
            DistanceMapReduceAlgebra<CircularArrayNode>, Dictionary<CircularArrayNode, int>>(nodes[startIndex]);

        var best = -1;

        foreach (var node in nodes)
        {
            if (node.Word != target || !distances.TryGetValue(node, out var distance))
            {
                continue;
            }

            if (best == -1 || distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    private static CircularArrayNode[] BuildCircularGraph(string[] words)
    {
        var n = words.Length;
        var nodes = new CircularArrayNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new CircularArrayNode(i, words[i]);
        }

        for (var i = 0; i < n; i++)
        {
            nodes[i].Neighbors.Add(nodes[(i + 1) % n]);
            nodes[i].Neighbors.Add(nodes[((i - 1) + n) % n]);
        }

        return nodes;
    }
}
