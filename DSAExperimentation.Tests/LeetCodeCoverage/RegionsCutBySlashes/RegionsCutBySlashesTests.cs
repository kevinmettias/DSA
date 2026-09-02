using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RegionsCutBySlashes;

// LeetCode 959. Regions Cut By Slashes: split each grid cell into 4 triangles
// (North/East/South/West), union the triangles a '/' or '\' character keeps on
// the same side of the cut, union every triangle when a cell is blank, then union
// each cell's East/South triangle with its neighbor's West/North triangle across
// the shared edge - the same DisjointSet-plus-Set "union everything connected,
// count distinct roots" shape NumberOfProvincesTests already proves, just over
// 4*n*n triangle ids instead of one id per grid cell.
public sealed partial class RegionsCutBySlashesTests
{
    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;

    [Fact]
    public void CountRegions_ClassicExample_TwoDiagonalSlashesFormOneRegion()
    {
        string[] grid = [" /", "/ "];

        Assert.Equal(2, CountRegions(grid));
    }

    [Fact]
    public void CountRegions_FourSlashesFormBackslashDiamond_ReturnsFiveRegions()
    {
        string[] grid = ["/\\", "\\/"];

        Assert.Equal(5, CountRegions(grid));
    }

    [Fact]
    public void CountRegions_AllBlankCells_ReturnsOneRegion()
    {
        string[] grid = ["  ", "  "];

        Assert.Equal(1, CountRegions(grid));
    }

    private static int CountRegions(string[] grid)
    {
        var size = grid.Length;
        var triangles = new DisjointSet(4 * size * size);

        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                UnionCellWithNeighbors(triangles, grid, r, c);
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < 4 * size * size; i++)
        {
            roots.TryAdd(triangles.Find(i));
        }

        return roots.Count;
    }

    // Unions the current cell's own triangles per its slash character, then unions
    // its East/South triangles with the West/North triangles of its right/below
    // neighbors across the shared edge.
    private static void UnionCellWithNeighbors(DisjointSet triangles, string[] grid, int r, int c)
    {
        var size = grid.Length;
        var baseId = 4 * (r * size + c);
        UnionWithinCell(triangles, baseId, grid[r][c]);

        if (c + 1 < size)
        {
            triangles.Union(baseId + East, 4 * (r * size + c + 1) + West);
        }

        if (r + 1 < size)
        {
            triangles.Union(baseId + South, 4 * ((r + 1) * size + c) + North);
        }
    }

    private static void UnionWithinCell(DisjointSet triangles, int baseId, char cell)
    {
        switch (cell)
        {
            case '/':
                triangles.Union(baseId + North, baseId + West);
                triangles.Union(baseId + East, baseId + South);
                break;
            case '\\':
                triangles.Union(baseId + North, baseId + East);
                triangles.Union(baseId + South, baseId + West);
                break;
            default:
                triangles.Union(baseId + North, baseId + East);
                triangles.Union(baseId + East, baseId + South);
                triangles.Union(baseId + South, baseId + West);
                break;
        }
    }
}
