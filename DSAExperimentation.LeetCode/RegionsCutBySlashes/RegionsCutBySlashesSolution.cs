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
                blocked[rowOffset, colOffset + BlockLastOffset] = true;
                blocked[rowOffset + 1, colOffset + 1] = true;
                blocked[rowOffset + BlockLastOffset, colOffset] = true;
                break;
            case '\\':
                blocked[rowOffset, colOffset] = true;
                blocked[rowOffset + 1, colOffset + 1] = true;
                blocked[rowOffset + BlockLastOffset, colOffset + BlockLastOffset] = true;
                break;
        }
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
        if (nextRow < 0 || nextRow >= grid.ExpandedSize || nextCol < 0 || nextCol >= grid.ExpandedSize)
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
    private static void UnionCellWithNeighbors(DisjointSet triangles, string[] grid, int r, int c)
    {
        var size = grid.Length;
        var baseId = TrianglesPerCell * (r * size + c);
        UnionWithinCell(triangles, baseId, grid[r][c]);

        if (c + 1 < size)
        {
            triangles.Union(baseId + East, TrianglesPerCell * (r * size + c + 1) + West);
        }

        if (r + 1 < size)
        {
            triangles.Union(baseId + South, TrianglesPerCell * ((r + 1) * size + c) + North);
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
