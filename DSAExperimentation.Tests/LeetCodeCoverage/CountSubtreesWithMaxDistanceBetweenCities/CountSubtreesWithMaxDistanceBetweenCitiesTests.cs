using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubtreesWithMaxDistanceBetweenCities;

// LeetCode 1617. Count Subtrees With Max Distance Between Cities: n <= 15, so every
// city subset is a bitmask, tried directly. A mask is a valid subtree iff a BFS from
// any one of its cities - this repo's own Queue<int> as the frontier, the same
// adjacency-list-plus-Queue shape MinimumHeightTreesTests/
// ShortestPathInBinaryMatrixTests already use for their own edge-list BFS - reaches
// every other city in the mask without ever stepping outside it. Its diameter is
// then the classic double-BFS trick: the second BFS, restarted from whichever city
// the first one found farthest, has its own farthest distance equal to the mask's
// diameter (valid because any connected subset of a tree is itself a tree).
public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesTests
{
    [Fact]
    public void CountSubtrees_StarShapedFourCityTree_MatchesKnownDistribution()
    {
        int[][] edges = [[1, 2], [2, 3], [2, 4]];

        var counts = CountSubtreesWithMaxDistance(4, edges);

        Assert.Equal([3, 4, 0], counts);
    }

    [Fact]
    public void CountSubtrees_SingleEdge_ReturnsOneSubtreeAtDistanceOne()
    {
        int[][] edges = [[1, 2]];

        var counts = CountSubtreesWithMaxDistance(2, edges);

        Assert.Equal([1], counts);
    }

    private static int[] CountSubtreesWithMaxDistance(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var counts = new int[n - 1];

        for (var mask = 1; mask < (1 << n); mask++)
        {
            if (PopCount(mask) < 2)
            {
                continue;
            }

            if (TryComputeDiameter(mask, n, adjacency, out var diameter))
            {
                counts[diameter - 1]++;
            }
        }

        return counts;
    }

    private static bool TryComputeDiameter(int mask, int n, List<int>[] adjacency, out int diameter)
    {
        var start = LowestSetBitIndex(mask);
        var firstPass = BfsDistances(start, mask, n, adjacency);

        if (!TryFindFarthest(mask, start, firstPass, out var farthest))
        {
            diameter = 0;
            return false;
        }

        var secondPass = BfsDistances(farthest, mask, n, adjacency);
        diameter = MaxDistanceInMask(mask, n, secondPass);
        return true;
    }

    private static bool TryFindFarthest(int mask, int start, int[] distances, out int farthest)
    {
        if (!AllReachable(mask, distances))
        {
            farthest = start;
            return false;
        }

        farthest = FindFarthestNode(mask, start, distances);
        return true;
    }

    private static bool AllReachable(int mask, int[] distances)
    {
        for (var node = 0; node < distances.Length; node++)
        {
            if ((mask & (1 << node)) != 0 && distances[node] == -1)
            {
                return false;
            }
        }

        return true;
    }

    private static int FindFarthestNode(int mask, int start, int[] distances)
    {
        var farthest = start;
        var maxDistance = 0;

        for (var node = 0; node < distances.Length; node++)
        {
            if ((mask & (1 << node)) != 0 && distances[node] > maxDistance)
            {
                maxDistance = distances[node];
                farthest = node;
            }
        }

        return farthest;
    }

    private static int MaxDistanceInMask(int mask, int n, int[] distances)
    {
        var diameter = 0;

        for (var node = 0; node < n; node++)
        {
            if ((mask & (1 << node)) != 0 && distances[node] > diameter)
            {
                diameter = distances[node];
            }
        }

        return diameter;
    }

    private static int[] BfsDistances(int start, int mask, int n, List<int>[] adjacency)
    {
        var distance = new int[n];
        Array.Fill(distance, -1);
        distance[start] = 0;

        var frontier = new RepoQueue();
        frontier.Enqueue(start);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in adjacency[node])
            {
                if ((mask & (1 << neighbor)) == 0 || distance[neighbor] != -1)
                {
                    continue;
                }

                distance[neighbor] = distance[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return distance;
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            var a = edge[0] - 1;
            var b = edge[1] - 1;
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency;
    }

    private static int PopCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }

    private static int LowestSetBitIndex(int mask)
    {
        var index = 0;

        while ((mask & (1 << index)) == 0)
        {
            index++;
        }

        return index;
    }
}
