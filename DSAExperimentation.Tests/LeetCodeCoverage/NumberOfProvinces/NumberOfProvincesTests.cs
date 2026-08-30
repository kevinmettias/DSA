using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfProvinces;

// LeetCode 547. Number of Provinces: union every pair the adjacency matrix marks
// connected into this repo's own DisjointSet, then count distinct roots with this
// repo's own Set<int> - the same DisjointSet RedundantConnectionTests already uses
// to detect a cycle-closing edge, just counting components at the end instead of
// stopping at the first edge that finds two nodes already joined.
public sealed partial class NumberOfProvincesTests
{
    [Fact]
    public void CountProvinces_ClassicExample_ReturnsTwoProvinces()
    {
        int[][] isConnected =
        [
            [1, 1, 0],
            [1, 1, 0],
            [0, 0, 1],
        ];

        Assert.Equal(2, CountProvinces(isConnected));
    }

    [Fact]
    public void CountProvinces_NoCityConnectedToAnother_ReturnsOneProvincePerCity()
    {
        int[][] isConnected =
        [
            [1, 0, 0],
            [0, 1, 0],
            [0, 0, 1],
        ];

        Assert.Equal(3, CountProvinces(isConnected));
    }

    [Fact]
    public void CountProvinces_TransitiveChain_MergesIntoSingleProvince()
    {
        // City 0 connects only to 1, and 1 only to 2 - no direct 0-2 edge, so the
        // province still has to be discovered transitively through Union's chaining.
        int[][] isConnected =
        [
            [1, 1, 0],
            [1, 1, 1],
            [0, 1, 1],
        ];

        Assert.Equal(1, CountProvinces(isConnected));
    }

    private static int CountProvinces(int[][] isConnected)
    {
        var cityCount = isConnected.Length;
        var components = new DisjointSet(cityCount);

        for (var i = 0; i < cityCount; i++)
        {
            for (var j = i + 1; j < cityCount; j++)
            {
                if (isConnected[i][j] == 1)
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < cityCount; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }
}
