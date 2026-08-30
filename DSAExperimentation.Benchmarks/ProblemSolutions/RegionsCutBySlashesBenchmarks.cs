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
        var expandedSize = GridSize * 3;
        var blocked = new bool[expandedSize, expandedSize];

        for (var r = 0; r < GridSize; r++)
        {
            for (var c = 0; c < GridSize; c++)
            {
                BlockDiagonal(blocked, r * 3, c * 3, _grid[r][c]);
            }
        }

        var visited = new bool[expandedSize, expandedSize];
        var regions = 0;

        for (var r = 0; r < expandedSize; r++)
        {
            for (var c = 0; c < expandedSize; c++)
            {
                if (blocked[r, c] || visited[r, c])
                {
                    continue;
                }

                FloodFill(blocked, visited, expandedSize, r, c);
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
                blocked[rowOffset, colOffset + 2] = true;
                blocked[rowOffset + 1, colOffset + 1] = true;
                blocked[rowOffset + 2, colOffset] = true;
                break;
            case '\\':
                blocked[rowOffset, colOffset] = true;
                blocked[rowOffset + 1, colOffset + 1] = true;
                blocked[rowOffset + 2, colOffset + 2] = true;
                break;
        }
    }

    private static void FloodFill(bool[,] blocked, bool[,] visited, int expandedSize, int startRow, int startCol)
    {
        var stack = new Stack<(int Row, int Col)>();
        stack.Push((startRow, startCol));
        visited[startRow, startCol] = true;

        Span<(int DeltaRow, int DeltaCol)> directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        while (stack.Count > 0)
        {
            var (row, col) = stack.Pop();

            foreach (var (deltaRow, deltaCol) in directions)
            {
                var nextRow = row + deltaRow;
                var nextCol = col + deltaCol;

                if (nextRow < 0 || nextRow >= expandedSize || nextCol < 0 || nextCol >= expandedSize)
                {
                    continue;
                }

                if (blocked[nextRow, nextCol] || visited[nextRow, nextCol])
                {
                    continue;
                }

                visited[nextRow, nextCol] = true;
                stack.Push((nextRow, nextCol));
            }
        }
    }

    [Benchmark]
    public int DisjointSetTriangleUnion()
    {
        var triangles = new DisjointSet(4 * GridSize * GridSize);

        for (var r = 0; r < GridSize; r++)
        {
            for (var c = 0; c < GridSize; c++)
            {
                var baseId = 4 * (r * GridSize + c);
                UnionWithinCell(triangles, baseId, _grid[r][c]);

                if (c + 1 < GridSize)
                {
                    triangles.Union(baseId + East, 4 * (r * GridSize + c + 1) + West);
                }

                if (r + 1 < GridSize)
                {
                    triangles.Union(baseId + South, 4 * ((r + 1) * GridSize + c) + North);
                }
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < 4 * GridSize * GridSize; i++)
        {
            roots.TryAdd(triangles.Find(i));
        }

        return roots.Count;
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
