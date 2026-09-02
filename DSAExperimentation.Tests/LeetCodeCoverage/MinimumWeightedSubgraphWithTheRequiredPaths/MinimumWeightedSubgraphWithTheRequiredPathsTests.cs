using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWeightedSubgraphWithTheRequiredPaths;

// LeetCode 2203. Minimum Weighted Subgraph With the Required Paths: the optimal
// subgraph is always two shortest paths (src1->meet, src2->meet) sharing one
// meeting-node suffix down to dest, so the answer is min over every candidate
// meeting node of dist(src1,node) + dist(src2,node) + dist(node,dest). That's three
// runs of this repo's own ShortestPath.Dijkstra - forward from src1, forward from
// src2, and one more on the edge-reversed graph from dest, since "distance to dest"
// is exactly "distance from dest on the reverse graph" - reused (NetworkDelayTime's
// own WeightedNode/WeightedTopology fixtures), not recomputed per candidate.
public sealed partial class MinimumWeightedSubgraphWithTheRequiredPathsTests
{
    [Fact]
    public void MinimumWeight_ClassicExample_ReturnsSumOfThreeShortestPaths()
    {
        var (forward, reverse) = BuildGraphs(5,
            [(0, 2, 2), (1, 2, 3), (2, 3, 1), (2, 4, 5), (3, 4, 2)]);

        var weight = MinimumWeight(forward, reverse, new PathEndpoints(Src1: 0, Src2: 1, Dest: 4));

        // Cheapest shared meeting node is 2: 0->2 (2) + 1->2 (3) + 2->3->4 (1+2) = 8,
        // beating a direct 2->4 edge (5) as the tail of the shared suffix.
        Assert.Equal(8, weight);
    }

    [Fact]
    public void MinimumWeight_DestUnreachableFromOneSource_ReturnsMinusOne()
    {
        var (forward, reverse) = BuildGraphs(3, [(0, 1, 1)]);

        var weight = MinimumWeight(forward, reverse, new PathEndpoints(Src1: 0, Src2: 2, Dest: 1));

        Assert.Equal(-1, weight);
    }

    private static (Dictionary<int, WeightedNode> Forward, Dictionary<int, WeightedNode> Reverse) BuildGraphs(
        int n, (int From, int To, int Weight)[] edges)
    {
        var forward = Enumerable.Range(0, n).ToDictionary(id => id, id => new WeightedNode(id.ToString()));
        var reverse = Enumerable.Range(0, n).ToDictionary(id => id, id => new WeightedNode(id.ToString()));

        foreach (var (from, to, weight) in edges)
        {
            forward[from].Edges.Add((weight, forward[to]));
            reverse[to].Edges.Add((weight, reverse[from]));
        }

        return (forward, reverse);
    }

    private readonly record struct PathEndpoints(int Src1, int Src2, int Dest);

    private static int MinimumWeight(
        Dictionary<int, WeightedNode> forward, Dictionary<int, WeightedNode> reverse, PathEndpoints endpoints)
    {
        var fromSrc1 = Dijkstra(forward[endpoints.Src1]);
        var fromSrc2 = Dijkstra(forward[endpoints.Src2]);
        var toDest = Dijkstra(reverse[endpoints.Dest]);

        var best = long.MaxValue;

        foreach (var id in forward.Keys)
        {
            if (fromSrc1.TryGetValue(forward[id], out var d1) &&
                fromSrc2.TryGetValue(forward[id], out var d2) &&
                toDest.TryGetValue(reverse[id], out var d3))
            {
                best = Math.Min(best, (long)d1 + d2 + d3);
            }
        }

        return best == long.MaxValue ? -1 : (int)best;
    }

    private static Dictionary<WeightedNode, int> Dijkstra(WeightedNode source)
        => ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(source);
}
