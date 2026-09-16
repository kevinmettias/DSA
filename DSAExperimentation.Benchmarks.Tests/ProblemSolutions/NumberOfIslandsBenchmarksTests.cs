using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfIslandsBenchmarks (ARCHITECTURE 17.9): it carries a single arm - this
// repo's depth-first sink - so there is nothing to agree with and the oracle is derived instead.
// Setup's workload comes from NumberOfIslandsWorkloads.BuildGrid, which is seeded and so reproducible:
// the same GridSize rebuilds the same cells. The expected island count is recomputed here by a
// breadth-first flood fill over that rebuilt grid - a different walk from the arm's depth-first sink,
// using an explicit queue and a visited array where the arm tracks a claimed set - so the assertion is
// not the arm restated. The lone arm carries no [Benchmark(Baseline = true)], which is not required.
public sealed partial class NumberOfIslandsBenchmarksTests
{
    private const int SmallestGridSize = 50;

    // The seed Setup hands NumberOfIslandsWorkloads.BuildGrid.
    private const int WorkloadSeed = 200;

    private const char Land = '1';

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DepthFirstSink(), BuildHarness().DepthFirstSink());

    [Fact]
    public void DepthFirstSink_SeededHalfLandGrid_CountsTheIndependentFloodFillTally() =>
        Assert.Equal(
            ExpectedIslandCount(
                NumberOfIslandsWorkloads.BuildGrid(SmallestGridSize, SmallestGridSize, WorkloadSeed)),
            BuildHarness().DepthFirstSink());

    // Breadth-first flood fill: every still-unvisited land cell starts exactly one island, and the
    // queue carries only that island's own cells out to their four neighbours. So the number of starts
    // is the island count, reached by a walk the arm does not take.
    private static int ExpectedIslandCount(char[][] grid)
    {
        var visited = new bool[grid.Length][];

        for (var row = 0; row < grid.Length; row++)
        {
            visited[row] = new bool[grid[row].Length];
        }

        var islands = 0;

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (visited[row][col] || grid[row][col] != Land)
                {
                    continue;
                }

                islands++;
                FloodFrom(grid, visited, row, col);
            }
        }

        return islands;
    }

    // One island's own flood: claim the start, then walk its four-neighbour frontier until it dries up.
    private static void FloodFrom(char[][] grid, bool[][] visited, int startRow, int startCol)
    {
        var frontier = new Queue<(int Row, int Col)>();
        visited[startRow][startCol] = true;
        frontier.Enqueue((startRow, startCol));

        while (frontier.Count > 0)
        {
            var (row, col) = frontier.Dequeue();
            OfferNeighbor(grid, visited, frontier, row - 1, col);
            OfferNeighbor(grid, visited, frontier, row + 1, col);
            OfferNeighbor(grid, visited, frontier, row, col - 1);
            OfferNeighbor(grid, visited, frontier, row, col + 1);
        }
    }

    // Queue a neighbour only when it is inside the grid and is unvisited land.
    private static void OfferNeighbor(
        char[][] grid, bool[][] visited, Queue<(int Row, int Col)> frontier, int row, int col)
    {
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length)
        {
            return;
        }

        if (visited[row][col] || grid[row][col] != Land)
        {
            return;
        }

        visited[row][col] = true;
        frontier.Enqueue((row, col));
    }

    private static NumberOfIslandsBenchmarks BuildHarness()
    {
        var harness = new NumberOfIslandsBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
