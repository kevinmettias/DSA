using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Regions Cut By Slashes (LC 959): the classic 3x3-subgrid expansion (baseline -
// blow each cell up into a 3x3 block with the slash's diagonal blocked, then
// flood-fill the resulting 9*n^2-cell grid with an explicit stack) vs. this
// repo's own DisjointSet unioning 4 triangles per cell (North/East/South/West)
// plus their cross-cell neighbors, counting distinct roots with this repo's own
// Set<int> - the same Union-Find primitive NumberOfProvincesBenchmarks already
// proves against a DFS flood-fill baseline, applied here to a finer-grained
// triangle graph instead of one node per grid cell.
[MemoryDiagnoser]
public class RegionsCutBySlashesBenchmarks
{
    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;
    private const int TrianglesPerCell = 4;
    private const int ExpansionFactor = 3;
    private const int BlockLastOffset = 2;
    private static readonly char[] SlashChars = [' ', '/', '\\'];

    [Params(30, 150)]
    public int GridSize;

    private char[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = new char[GridSize][];

        for (var r = 0; r < GridSize; r++)
        {
            _grid[r] = new char[GridSize];
            for (var c = 0; c < GridSize; c++)
            {
                _grid[r][c] = SlashChars[random.Next(SlashChars.Length)];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int ThreeByThreeExpansionFloodFill()
    {
        var expandedSize = GridSize * ExpansionFactor;
        var blocked = BuildBlockedGrid(expandedSize);

        return CountFloodFillRegions(blocked, expandedSize);
    }

    private bool[,] BuildBlockedGrid(int expandedSize)
    {
        var blocked = new bool[expandedSize, expandedSize];

        for (var r = 0; r < GridSize; r++)
        {
            for (var c = 0; c < GridSize; c++)
            {
                BlockDiagonal(blocked, r * ExpansionFactor, c * ExpansionFactor, _grid[r][c]);
            }
        }

        return blocked;
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

    [Benchmark]
    public int DisjointSetTriangleUnion()
    {
        var triangles = new DisjointSet(TrianglesPerCell * GridSize * GridSize);

        for (var r = 0; r < GridSize; r++)
        {
            for (var c = 0; c < GridSize; c++)
            {
                UnionCellWithNeighbors(triangles, r, c, _grid[r][c]);
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < TrianglesPerCell * GridSize * GridSize; i++)
        {
            roots.TryAdd(triangles.Find(i));
        }

        return roots.Count;
    }

    private void UnionCellWithNeighbors(DisjointSet triangles, int r, int c, char cell)
    {
        var baseId = TrianglesPerCell * (r * GridSize + c);
        UnionWithinCell(triangles, baseId, cell);

        if (c + 1 < GridSize)
        {
            triangles.Union(baseId + East, TrianglesPerCell * (r * GridSize + c + 1) + West);
        }

        if (r + 1 < GridSize)
        {
            triangles.Union(baseId + South, TrianglesPerCell * ((r + 1) * GridSize + c) + North);
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
