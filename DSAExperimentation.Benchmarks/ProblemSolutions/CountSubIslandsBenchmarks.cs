using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Sub Islands (LC 1905): a hand-specialized recursive flood fill (the textbook
// approach - walk each grid2 island, tracking along the way whether every visited
// cell is also land in grid1) vs. this repo's own DepthFirstSearch.Traverse walking
// each island's reachable cells into a list first and checking the subset relation
// against grid1 afterward - the same border-flood-fill primitive
// MaxAreaOfIslandBenchmarks already uses for LC 695.
[MemoryDiagnoser]
public class CountSubIslandsBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private const int RandomSeed = 1905; // LC 1905 problem number
    private const double Grid1LandProbability = 0.7;
    private const double Grid2LandProbability = 0.55;

    [Params(30, 120)]
    public int Side;

    private int[][] _grid1 = null!;
    private int[][] _grid2 = null!;

    private readonly record struct GridBounds(int Rows, int Cols);

    private readonly record struct GridPosition(int Row, int Col);

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid1 = new int[Side][];
        _grid2 = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid1[r] = new int[Side];
            _grid2[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                // grid1 is land-biased relative to grid2 so a real mix of grid2
                // islands both qualify and fail to qualify as sub-islands.
                _grid1[r][c] = random.NextDouble() < Grid1LandProbability ? 1 : 0;
                _grid2[r][c] = random.NextDouble() < Grid2LandProbability ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var grid2 = CloneGrid(_grid2);
        var bounds = new GridBounds(grid2.Length, grid2[0].Length);
        var count = 0;

        ForEachCell(bounds, position => ProcessCellFlood(grid2, position, bounds, ref count));

        return count;
    }

    private static void ForEachCell(GridBounds bounds, Action<GridPosition> visit)
    {
        for (var r = 0; r < bounds.Rows; r++)
        {
            for (var c = 0; c < bounds.Cols; c++)
            {
                visit(new GridPosition(r, c));
            }
        }
    }

    private void ProcessCellFlood(int[][] grid2, GridPosition position, GridBounds bounds, ref int count)
    {
        if (grid2[position.Row][position.Col] != 1)
        {
            return;
        }

        var isSubIsland = true;
        Flood(grid2, position, bounds, ref isSubIsland);

        if (isSubIsland)
        {
            count++;
        }
    }

    private void Flood(int[][] grid2, GridPosition position, GridBounds bounds, ref bool isSubIsland)
    {
        var row = position.Row;
        var col = position.Col;

        if (row < 0 || row >= bounds.Rows || col < 0 || col >= bounds.Cols || grid2[row][col] != 1)
        {
            return;
        }

        grid2[row][col] = 0;

        if (_grid1[row][col] != 1)
        {
            isSubIsland = false;
        }

        Flood(grid2, new GridPosition(row + 1, col), bounds, ref isSubIsland);
        Flood(grid2, new GridPosition(row - 1, col), bounds, ref isSubIsland);
        Flood(grid2, new GridPosition(row, col + 1), bounds, ref isSubIsland);
        Flood(grid2, new GridPosition(row, col - 1), bounds, ref isSubIsland);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var grid2 = CloneGrid(_grid2);
        var bounds = new GridBounds(grid2.Length, grid2[0].Length);
        var count = 0;

        ForEachCell(bounds, position => ProcessCellDfs(grid2, position, bounds, ref count));

        return count;
    }

    private void ProcessCellDfs(int[][] grid2, GridPosition position, GridBounds bounds, ref int count)
    {
        if (grid2[position.Row][position.Col] != 1)
        {
            return;
        }

        var island = TraverseIsland(grid2, bounds, position);
        var isSubIsland = MarkIslandVisited(grid2, island);

        if (isSubIsland)
        {
            count++;
        }
    }

    private static List<(int Row, int Col)> TraverseIsland(int[][] grid2, GridBounds bounds, GridPosition position)
    {
        return DepthFirstSearch.Traverse((position.Row, position.Col), p => Neighbors(grid2, bounds, p));
    }

    private bool MarkIslandVisited(int[][] grid2, List<(int Row, int Col)> island)
    {
        var isSubIsland = true;

        foreach (var (row, col) in island)
        {
            if (_grid1[row][col] != 1)
            {
                isSubIsland = false;
            }

            grid2[row][col] = 0;
        }

        return isSubIsland;
    }

    private static IEnumerable<(int Row, int Col)> Neighbors(int[][] grid2, GridBounds bounds, (int Row, int Col) position)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = position.Row + dRow;
            var nextCol = position.Col + dCol;

            if (IsWalkableLand(grid2, bounds, nextRow, nextCol))
            {
                yield return (nextRow, nextCol);
            }
        }
    }

    private static bool IsWalkableLand(int[][] grid2, GridBounds bounds, int row, int col)
    {
        if (row < 0 || row >= bounds.Rows || col < 0 || col >= bounds.Cols)
        {
            return false;
        }

        return grid2[row][col] == 1;
    }

    private static int[][] CloneGrid(int[][] source)
    {
        var clone = new int[source.Length][];

        for (var r = 0; r < source.Length; r++)
        {
            clone[r] = (int[])source[r].Clone();
        }

        return clone;
    }
}
