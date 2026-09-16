using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.RegionsCutBySlashes;

// LeetCode 959. Regions Cut By Slashes: count the contiguous regions an n x n
// grid of '/', '\' and ' ' cells is cut into.
//
// A slash cuts a cell diagonally, so a cell is not the right unit of
// connectivity - the two strategies differ only in how they refine it. The
// baseline blows each cell up into a 3x3 block with the diagonal blocked out and
// flood-fills the resulting 9n^2-cell grid; the composed arm splits each cell
// into its four North/East/South/West triangles instead and unions the ones the
// cut leaves on the same side, then unions each cell's East/South triangle with
// its neighbor's West/North triangle across the shared edge. That is the same
// DisjointSet-plus-Set "union everything connected, count distinct roots" shape
// NumberOfProvincesSolution uses for LC 547, over 4n^2 triangle ids instead of
// one id per vertex.
internal static class RegionsCutBySlashesSolution
{
    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;
    private const int TrianglesPerCell = 4;

    // The 3x3 block the baseline expands each cell into, and the offset of that
    // block's last row/column.
    private const int ExpansionFactor = 3;
    private const int BlockLastOffset = 2;

    // The textbook answer: expand to a 3x3 block per cell, mark the slash's own
    // diagonal as wall, then flood-fill the open squares with an explicit stack.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int CountRegionsByExpandedFloodFill(string[] grid)
    {
        var expandedSize = grid.Length * ExpansionFactor;
        var blocked = BuildBlockedGrid(grid, expandedSize);

        return CountFloodFillRegions(blocked, expandedSize);
    }

    private static bool[,] BuildBlockedGrid(string[] grid, int expandedSize)
    {
        var blocked = new bool[expandedSize, expandedSize];

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid.Length; c++)
            {
                BlockDiagonal(blocked, r * ExpansionFactor, c * ExpansionFactor, grid[r][c]);
            }
        }

        return blocked;
    }

    private static void BlockDiagonal(bool[,] blocked, int rowOffset, int colOffset, char cell)
    {
        switch (cell)
        {
            case '/':
                BlockForwardSlashDiagonal(blocked, rowOffset, colOffset);
                break;
            case '\\':
                BlockBackSlashDiagonal(blocked, rowOffset, colOffset);
                break;
        }
    }

    // '/' runs from the block's top-right corner down to its bottom-left, so it
    // walls those two corners and the centre the diagonal passes through.
    private static void BlockForwardSlashDiagonal(bool[,] blocked, int rowOffset, int colOffset)
    {
        blocked[rowOffset, colOffset + BlockLastOffset] = true;
        blocked[rowOffset + 1, colOffset + 1] = true;
        blocked[rowOffset + BlockLastOffset, colOffset] = true;
    }

    // '\' mirrors that: top-left, centre, bottom-right.
    private static void BlockBackSlashDiagonal(bool[,] blocked, int rowOffset, int colOffset)
    {
        blocked[rowOffset, colOffset] = true;
        blocked[rowOffset + 1, colOffset + 1] = true;
        blocked[rowOffset + BlockLastOffset, colOffset + BlockLastOffset] = true;
    }

    private static int CountFloodFillRegions(bool[,] blocked, int expandedSize)
    {
        var visited = new bool[expandedSize, expandedSize];
        var grid = new FloodFillGrid(blocked, visited, expandedSize);
        var regions = 0;

        for (var r = 0; r < expandedSize; r++)
        {
            for (var c = 0; c < expandedSize; c++)
            {
                if (blocked[r, c] || visited[r, c])
                {
                    continue;
                }

                FloodFill(grid, r, c);
                regions++;
            }
        }

        return regions;
    }

    private readonly record struct FloodFillGrid(bool[,] Blocked, bool[,] Visited, int ExpandedSize);

    private static void FloodFill(FloodFillGrid grid, int startRow, int startCol)
    {
        var stack = new Stack<(int Row, int Col)>();
        stack.Push((startRow, startCol));
        grid.Visited[startRow, startCol] = true;

        Span<(int DeltaRow, int DeltaCol)> directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        while (stack.Count > 0)
        {
            var (row, col) = stack.Pop();

            foreach (var (deltaRow, deltaCol) in directions)
            {
                VisitNeighborIfOpen(grid, stack, row + deltaRow, col + deltaCol);
            }
        }
    }

    private static void VisitNeighborIfOpen(FloodFillGrid grid, Stack<(int Row, int Col)> stack, int nextRow, int nextCol)
    {
        if (!IsOnGrid(grid, nextRow, nextCol))
        {
            return;
        }

        if (grid.Blocked[nextRow, nextCol] || grid.Visited[nextRow, nextCol])
        {
            return;
        }

        grid.Visited[nextRow, nextCol] = true;
        stack.Push((nextRow, nextCol));
    }

    // Whether the neighbor lies on the expanded grid at all.
    private static bool IsOnGrid(FloodFillGrid grid, int row, int col)
        => row >= 0 && row < grid.ExpandedSize && col >= 0 && col < grid.ExpandedSize;

    // Four triangles per cell in this repo's own DisjointSet, unioned according to
    // the cut and then across every shared edge; the distinct roots this repo's
    // own Set<int> collects are the regions.
    public static int CountRegionsByDisjointSetTriangles(string[] grid)
    {
        var size = grid.Length;
        var triangleCount = TrianglesPerCell * size * size;
        var triangles = new DisjointSet(triangleCount);

        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                UnionCellWithNeighbors(triangles, grid, r, c);
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < triangleCount; i++)
        {
            roots.TryAdd(triangles.Find(i));
        }

        return roots.Count;
    }

    // Unions the current cell's own triangles per its slash character, then unions
    // its East/South triangles with the West/North triangles of its right/below
    // neighbors across the shared edge.
    private static void UnionCellWithNeighbors(DisjointSet triangles, string[] grid, int rowIndex, int columnIndex)
    {
        var size = grid.Length;
        var baseId = TrianglesPerCell * (rowIndex * size + columnIndex);
        UnionWithinCell(triangles, baseId, grid[rowIndex][columnIndex]);

        UnionWithNeighborCells(triangles, baseId, size, (rowIndex, columnIndex));
    }

    private static void UnionWithinCell(DisjointSet triangles, int baseId, char cell)
    {
        switch (cell)
        {
            case '/':
                UnionTrianglesCutByForwardSlash(triangles, baseId);
                break;
            case '\\':
                UnionTrianglesCutByBackSlash(triangles, baseId);
                break;
            default:
                UnionTrianglesOfBlankCell(triangles, baseId);
                break;
        }
    }

    // '/' separates the North/West triangles from the East/South ones.
    private static void UnionTrianglesCutByForwardSlash(DisjointSet triangles, int baseId)
    {
        triangles.Union(baseId + North, baseId + West);
        triangles.Union(baseId + East, baseId + South);
    }

    // '\' separates North/East from South/West instead.
    private static void UnionTrianglesCutByBackSlash(DisjointSet triangles, int baseId)
    {
        triangles.Union(baseId + North, baseId + East);
        triangles.Union(baseId + South, baseId + West);
    }

    // A blank cell is uncut, so all four triangles stay one region. Two unions
    // chained through East and South reach all four without a third lookup.
    private static void UnionTrianglesOfBlankCell(DisjointSet triangles, int baseId)
    {
        triangles.Union(baseId + North, baseId + East);
        triangles.Union(baseId + East, baseId + South);
        triangles.Union(baseId + South, baseId + West);
    }

    // Unions this cell's East/South triangle with the West/North triangle of its right
    // and below neighbours along the shared edge - the cross-cell half of the union
    // above.
    private static void UnionWithNeighborCells(
        DisjointSet triangles, int baseId, int size, (int Row, int Col) cell)
    {
        if (cell.Col + 1 < size)
        {
            triangles.Union(baseId + East, TrianglesPerCell * (cell.Row * size + cell.Col + 1) + West);
        }

        if (cell.Row + 1 < size)
        {
            triangles.Union(baseId + South, TrianglesPerCell * ((cell.Row + 1) * size + cell.Col) + North);
        }
    }
}
