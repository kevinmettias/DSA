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
        var walk = new FloodWalk(grid, visited, frontier);
        visited[startRow][startCol] = true;
        frontier.Enqueue((startRow, startCol));

        while (frontier.Count > 0)
        {
            var (row, col) = frontier.Dequeue();
            OfferNeighbor(walk, row - 1, col);
            OfferNeighbor(walk, row + 1, col);
            OfferNeighbor(walk, row, col - 1);
            OfferNeighbor(walk, row, col + 1);
        }
    }

    // Queue a neighbour only when it is inside the grid and is unvisited land. The three values every
    // offer shares - the grid, its marks and the frontier - travel together as one walk, so a call
    // site names only the cell it is offering.
    private static void OfferNeighbor(FloodWalk walk, int row, int col)
    {
        var isOffGrid = row < 0 || row >= walk.Grid.Length || col < 0 || col >= walk.Grid[row].Length;

        if (isOffGrid)
        {
            return;
        }

        if (walk.Visited[row][col] || walk.Grid[row][col] != Land)
        {
            return;
        }

        walk.Visited[row][col] = true;
        walk.Frontier.Enqueue((row, col));
    }

    private static NumberOfIslandsBenchmarks BuildHarness()
    {
        var harness = new NumberOfIslandsBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }

    // One island's flood in progress: the grid, the marks the walk leaves and the frontier it is
    // draining. Data only - the walk's steps stay as static methods on the test class.
    private readonly record struct FloodWalk(
        char[][] Grid, bool[][] Visited, Queue<(int Row, int Col)> Frontier);
}
