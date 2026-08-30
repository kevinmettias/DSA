using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostStonesRemovedWithSameRowOrColumn;

// LeetCode 947. Most Stones Removed with Same Row or Column: union each stone's row
// id with its column id (columns offset past every row id so the two axes share one
// DisjointSet universe without colliding, the same "encode two id spaces into one
// dense-int universe" move RedundantConnectionTests' nodeCount+1 sizing already makes
// for a single axis). Every connected component can be reduced to one surviving
// stone by repeatedly removing a stone that still shares a row/column with another
// remaining stone, so the answer is stones.Length minus the number of components.
public sealed partial class MostStonesRemovedWithSameRowOrColumnTests
{
    [Fact]
    public void RemoveStones_ClassicGrid_RemovesAllButOnePerComponent()
    {
        int[][] stones = [[0, 0], [0, 1], [1, 0], [1, 2], [2, 1], [2, 2]];

        var removed = RemoveStones(stones);

        Assert.Equal(5, removed);
    }

    [Fact]
    public void RemoveStones_NoSharedRowsOrColumns_RemovesNone()
    {
        int[][] stones = [[0, 0], [1, 1], [2, 2]];

        var removed = RemoveStones(stones);

        Assert.Equal(0, removed);
    }

    private static int RemoveStones(int[][] stones)
    {
        var colOffset = stones.Max(stone => stone[0]) + 1;
        var universeSize = colOffset + stones.Max(stone => stone[1]) + 1;
        var components = new DisjointSet(universeSize);

        foreach (var stone in stones)
        {
            components.Union(stone[0], colOffset + stone[1]);
        }

        var roots = new HashSet<int>();
        foreach (var stone in stones)
        {
            roots.Add(components.Find(stone[0]));
        }

        return stones.Length - roots.Count;
    }
}
