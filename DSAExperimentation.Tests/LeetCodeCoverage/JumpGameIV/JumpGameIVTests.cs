using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIV;

// LeetCode 1345. Jump Game IV: minimum steps from index 0 to the last index, where a
// step goes to i+1, i-1, or any index sharing arr[i]'s value. Modeled as an implicit
// unweighted-hop graph (same JumpGameII/WeightedNode fixture this repo already uses
// for that problem) and answered with this repo's own ShortestPath.Dijkstra - not
// the textbook per-value-group BFS with a "clear the group after first use" trick.
public sealed partial class JumpGameIVTests
{
    [Theory]
    [InlineData(new[] { 100, -23, -23, 404, 100, 23, 23, 23, 3, 404 }, 3)]
    [InlineData(new[] { 7 }, 0)]
    [InlineData(new[] { 7, 6, 9, 6, 9, 6, 9, 7 }, 1)]
    [InlineData(new[] { 6, 1, 9 }, 2)]
    public void MinJumps_DijkstraOverImplicitHopGraph_ReturnsMinimumStepCount(int[] arr, int expected)
        => Assert.Equal(expected, MinJumps(arr));

    private static int MinJumps(int[] arr)
    {
        var nodes = BuildHopGraph(arr);

        var distances = ShortestPath.Dijkstra<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }

    private static WeightedNode[] BuildHopGraph(int[] arr)
    {
        var nodes = new WeightedNode[arr.Length];
        for (var i = 0; i < arr.Length; i++)
        {
            nodes[i] = new WeightedNode(i.ToString());
        }

        var indicesByValue = new Dictionary<int, List<int>>();
        for (var i = 0; i < arr.Length; i++)
        {
            if (!indicesByValue.TryGetValue(arr[i], out var indices))
            {
                indices = [];
                indicesByValue[arr[i]] = indices;
            }

            indices.Add(i);
        }

        for (var i = 0; i < arr.Length; i++)
        {
            if (i + 1 < arr.Length)
            {
                nodes[i].Edges.Add((1, nodes[i + 1]));
            }

            if (i - 1 >= 0)
            {
                nodes[i].Edges.Add((1, nodes[i - 1]));
            }

            foreach (var j in indicesByValue[arr[i]])
            {
                if (j != i)
                {
                    nodes[i].Edges.Add((1, nodes[j]));
                }
            }
        }

        return nodes;
    }
}
