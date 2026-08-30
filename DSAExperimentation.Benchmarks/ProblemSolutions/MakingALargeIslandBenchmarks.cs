using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Making A Large Island (LC 827): repeatedly flipping each water cell and running
// a fresh recursive flood fill from scratch (the textbook brute force) vs. this
// repo's own DepthFirstSearch.Traverse labeling every island exactly once - the
// same primitive MaxAreaOfIslandBenchmarks uses for LC 695 - into a
// HashMap<int,int> of id -> area, then summing each water cell's already-known
// neighboring areas (deduped through this repo's own Set<int>) in a single pass.
[MemoryDiagnoser]
public class MakingALargeIslandBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(20, 60)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _grid[r][c] = random.NextDouble() < 0.6 ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRepeatedFloodFill()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                grid[r][c] = 1;
                var visited = new bool[rows, cols];
                best = Math.Max(best, FloodCount(grid, visited, r, c, rows, cols));
                grid[r][c] = 0;
            }
        }

        return best;
    }

    private static int FloodCount(int[][] grid, bool[,] visited, int row, int col, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || visited[row, col] || grid[row][col] != 1)
        {
            return 0;
        }

        visited[row, col] = true;

        return 1
            + FloodCount(grid, visited, row + 1, col, rows, cols)
            + FloodCount(grid, visited, row - 1, col, rows, cols)
            + FloodCount(grid, visited, row, col + 1, rows, cols)
            + FloodCount(grid, visited, row, col - 1, rows, cols);
    }

    [Benchmark]
    public int LabeledFloodFillWithAreaLookup()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var areaById = new HashMap<int, int>();
        var nextId = 2;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 1)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), Neighbors);
                areaById.Set(nextId, island.Count);

                foreach (var (row, col) in island)
                {
                    grid[row][col] = nextId;
                }

                nextId++;
            }
        }

        var best = 0;

        foreach (var area in areaById.Values)
        {
            best = Math.Max(best, area);
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                var seenIslandIds = new Set<int>();
                var merged = 1;

                foreach (var (dRow, dCol) in Directions)
                {
                    var nextRow = r + dRow;
                    var nextCol = c + dCol;

                    if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                    {
                        continue;
                    }

                    var neighborId = grid[nextRow][nextCol];

                    if (neighborId < 2 || !seenIslandIds.TryAdd(neighborId))
                    {
                        continue;
                    }

                    if (areaById.TryGetValue(neighborId, out var area))
                    {
                        merged += area;
                    }
                }

                best = Math.Max(best, merged);
            }
        }

        return best;

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }

    private int[][] CloneGrid()
    {
        var clone = new int[_grid.Length][];

        for (var r = 0; r < _grid.Length; r++)
        {
            clone[r] = (int[])_grid[r].Clone();
        }

        return clone;
    }
}
