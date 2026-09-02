using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumScoreOfAPathBetweenTwoCities;

// LeetCode 2492. Minimum Score of a Path Between Two Cities: since a path may revisit
// roads/cities freely, the minimum score reachable between city 1 and city n is just
// the minimum weight among ALL roads in the connected component containing city 1 (the
// problem guarantees city n is in that same component). This repo's own DisjointSet
// unions every road's endpoints - the same Union-Find-for-component-membership move
// GraphConnectivityWithThresholdTests/NumberOfProvincesTests already use - then one
// more pass over the roads keeps the minimum weight among only those whose endpoint
// shares city 1's root, ignoring any road that lives in an unrelated component.
public sealed partial class MinimumScoreOfAPathBetweenTwoCitiesTests
{
    [Fact]
    public void MinScore_ClassicExampleOne_ReturnsMinimumEdgeWeightInCity1Component()
    {
        int[][] roads = [[1, 2, 9], [2, 3, 6], [2, 4, 5], [1, 4, 7]];

        Assert.Equal(5, MinScore(4, roads));
    }

    [Fact]
    public void MinScore_ClassicExampleTwo_ReturnsMinimumEdgeWeightInCity1Component()
    {
        int[][] roads = [[1, 2, 2], [1, 3, 4], [3, 4, 7]];

        Assert.Equal(2, MinScore(4, roads));
    }

    [Fact]
    public void MinScore_UnrelatedComponentHasSmallerWeight_IsIgnored()
    {
        // Cities 5-6 form a cheaper, disconnected component that must not affect the
        // answer for the component actually containing city 1 and city 4.
        int[][] roads = [[1, 2, 9], [2, 3, 6], [2, 4, 5], [1, 4, 7], [5, 6, 1]];

        Assert.Equal(5, MinScore(6, roads));
    }

    private static int MinScore(int n, int[][] roads)
    {
        var components = new DisjointSet(n + 1);

        foreach (var road in roads)
        {
            components.Union(road[0], road[1]);
        }

        var targetRoot = components.Find(1);
        var minScore = int.MaxValue;

        foreach (var road in roads)
        {
            if (components.Find(road[0]) == targetRoot)
            {
                minScore = Math.Min(minScore, road[2]);
            }
        }

        return minScore;
    }
}
