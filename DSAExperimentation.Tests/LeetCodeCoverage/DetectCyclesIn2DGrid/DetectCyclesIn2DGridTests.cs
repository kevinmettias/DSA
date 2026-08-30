using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetectCyclesIn2DGrid;

// LeetCode 1559. Detect Cycles in 2D Grid: union each cell with its same-character
// right/down neighbor exactly once per edge - the same "already connected before
// union means a cycle" shape RedundantConnectionTests/RegionsCutBySlashesTests
// already prove over this repo's own DisjointSet, gated here on same-character
// adjacency. Visiting only right/down per cell (never left/up) means every union
// crosses a genuinely new edge, so IsConnected can only already be true when a path
// of length >= 3 closes back into the same component - a cycle of length >= 4,
// exactly this problem's own minimum, satisfied for free rather than checked
// separately.
public sealed partial class DetectCyclesIn2DGridTests
{
    [Fact]
    public void ContainsCycle_RingOfAsAroundRingOfBs_ReturnsTrue()
    {
        string[] grid = ["aaaa", "abba", "abba", "aaaa"];

        Assert.True(ContainsCycle(grid));
    }

    [Fact]
    public void ContainsCycle_CRingAroundIsolatedCharacters_ReturnsTrue()
    {
        string[] grid = ["ccca", "cdcc", "ccec", "fccc"];

        Assert.True(ContainsCycle(grid));
    }

    [Fact]
    public void ContainsCycle_NoRepeatedComponentEdge_ReturnsFalse()
    {
        string[] grid = ["abb", "bzb", "bbb"];

        Assert.False(ContainsCycle(grid));
    }

    private static bool ContainsCycle(string[] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var components = new DisjointSet(rows * cols);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (HasCycleThroughNeighbor(grid, components, cols, (row, col), (row, col + 1)))
                {
                    return true;
                }

                if (HasCycleThroughNeighbor(grid, components, cols, (row, col), (row + 1, col)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasCycleThroughNeighbor(
        string[] grid, DisjointSet components, int cols, (int Row, int Col) cell, (int Row, int Col) neighbor)
    {
        if (neighbor.Row >= grid.Length || neighbor.Col >= cols
            || grid[neighbor.Row][neighbor.Col] != grid[cell.Row][cell.Col])
        {
            return false;
        }

        var id = (cell.Row * cols) + cell.Col;
        var neighborId = (neighbor.Row * cols) + neighbor.Col;

        if (components.IsConnected(id, neighborId))
        {
            return true;
        }

        components.Union(id, neighborId);
        return false;
    }
}
