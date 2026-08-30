using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameII;

// LeetCode 45. Jump Game II: minimum jumps to reach the last index, modeled as an
// implicit unweighted-hop graph (index i has an edge to every index reachable in one
// jump) and answered with this repo's own ShortestPath.Dijkstra - not the textbook
// O(n) greedy two-pointer scan.
public sealed partial class JumpGameIITests
{
    [Theory]
    [InlineData(new[] { 2, 3, 1, 1, 4 }, 2)]
    [InlineData(new[] { 2, 3, 0, 1, 4 }, 2)]
    [InlineData(new[] { 0 }, 0)]
    public void MinJumps_DijkstraOverImplicitHopGraph_ReturnsMinimumJumpCount(int[] nums, int expected)
        => Assert.Equal(expected, MinJumps(nums));

    private static int MinJumps(int[] nums)
    {
        var nodes = BuildHopGraph(nums);

        var distances = ShortestPath.Dijkstra<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }

    private static WeightedNode[] BuildHopGraph(int[] nums)
    {
        var nodes = new WeightedNode[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            nodes[i] = new WeightedNode(i.ToString());
        }

        for (var i = 0; i < nums.Length; i++)
        {
            var reach = Math.Min(i + nums[i], nums.Length - 1);

            for (var j = i + 1; j <= reach; j++)
            {
                nodes[i].Edges.Add((1, nodes[j]));
            }
        }

        return nodes;
    }
}
