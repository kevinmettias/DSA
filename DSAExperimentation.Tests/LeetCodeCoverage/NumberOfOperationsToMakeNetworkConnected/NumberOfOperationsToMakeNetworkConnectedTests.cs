using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfOperationsToMakeNetworkConnected;

// LeetCode 1319. Number of Operations to Make Network Connected: union every
// connection into this repo's own DisjointSet (NumberOfProvincesTests precedent),
// then read the answer off as componentCount - 1 - the minimum number of cables
// that must be moved to connect every remaining isolated component, since one
// spare cable buys exactly one merge. -1 whenever there simply aren't enough
// cables (fewer than n-1) to connect n computers at all, checked before touching
// the DisjointSet.
public sealed partial class NumberOfOperationsToMakeNetworkConnectedTests
{
    [Fact]
    public void MakeConnected_ClassicExample_ReturnsOneOperation()
    {
        int[][] connections = [[0, 1], [0, 2], [1, 2]];

        Assert.Equal(1, MakeConnected(4, connections));
    }

    [Fact]
    public void MakeConnected_TwoOperationsNeeded_ReturnsTwo()
    {
        int[][] connections = [[0, 1], [0, 2], [0, 3], [1, 2], [1, 3]];

        Assert.Equal(2, MakeConnected(6, connections));
    }

    [Fact]
    public void MakeConnected_NotEnoughCables_ReturnsNegativeOne()
    {
        int[][] connections = [[0, 1], [0, 2], [0, 3], [1, 2]];

        Assert.Equal(-1, MakeConnected(6, connections));
    }

    [Fact]
    public void MakeConnected_AlreadyFullyConnected_ReturnsZero()
    {
        int[][] connections = [[0, 1], [1, 2], [2, 3]];

        Assert.Equal(0, MakeConnected(4, connections));
    }

    private static int MakeConnected(int n, int[][] connections)
    {
        if (connections.Length < n - 1)
        {
            return -1;
        }

        var components = new DisjointSet(n);
        foreach (var connection in connections)
        {
            components.Union(connection[0], connection[1]);
        }

        var roots = new Set<int>();
        for (var i = 0; i < n; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count - 1;
    }
}
