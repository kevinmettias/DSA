using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// LeetCode 1334. Find the City With the Smallest Number of Neighbors at a
// Threshold Distance: wire every (undirected) road both ways onto this repo's
// own WeightedNode/WeightedTopology fixtures, then read every pair's shortest
// distance off AllPairsShortestPaths.TryComputeDistances (Floyd-Warshall) -
// AllPairsShortestPathsTests' own fixtures, here answering "how many cities are
// reachable within the threshold" for every city from one all-pairs matrix
// instead of one (from,to) lookup at a time. Ties favor the larger city id by
// scanning ascending and only replacing the best candidate on a <=.
public sealed partial class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceTests
{
    [Fact]
    public void FindCity_ClassicExample_ReturnsCityWithFewestReachableNeighbors()
    {
        int[][] edges = [[0, 1, 3], [1, 2, 1], [1, 3, 4], [2, 3, 1]];

        Assert.Equal(3, FindCity(4, edges, distanceThreshold: 4));
    }

    [Fact]
    public void FindCity_SecondExample_ReturnsCityZero()
    {
        int[][] edges = [[0, 1, 2], [0, 4, 8], [1, 2, 3], [1, 4, 2], [2, 3, 1], [3, 4, 1]];

        Assert.Equal(0, FindCity(5, edges, distanceThreshold: 2));
    }

    private static int FindCity(int n, int[][] edges, int distanceThreshold)
    {
        var nodes = new Dictionary<int, WeightedNode>();
        for (var i = 0; i < n; i++)
        {
            nodes[i] = new WeightedNode(i.ToString());
        }

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[0], edge[1], edge[2]);
            nodes[from].Edges.Add((weight, nodes[to]));
            nodes[to].Edges.Add((weight, nodes[from]));
        }

        AllPairsShortestPaths.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            nodes.Values, out var distances);

        var bestCity = -1;
        var bestCount = int.MaxValue;

        for (var city = 0; city < n; city++)
        {
            var count = 0;

            for (var other = 0; other < n; other++)
            {
                if (other != city
                    && distances.TryGetValue((nodes[city], nodes[other]), out var distance)
                    && distance <= distanceThreshold)
                {
                    count++;
                }
            }

            if (count <= bestCount)
            {
                bestCount = count;
                bestCity = city;
            }
        }

        return bestCity;
    }
}
