using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SwimInRisingWaterBenchmarks (ARCHITECTURE 17.9): both arms are
// SwimInRisingWaterSolution's - the binary search over elevations with a flood fill at each probe
// against the heap Dijkstra over the same grid - so a harness whose arms disagree is timing two
// different questions. Both answer with a bare int.
//
// The grid is decisive rather than merely agreed: [GlobalSetup] lays a random permutation of
// 0..Size*Size-1 out row by row, so the answer is the smallest elevation at which the two corners are
// joined through cells no higher than it. That is restated below by scanning the elevations in order
// and testing each with its own breadth-first walk over a mirrored copy of the grid, which shares no
// traversal with either arm.
public sealed partial class SwimInRisingWaterBenchmarksTests
{
    // Mirrors SwimInRisingWaterBenchmarks' own private RandomSeed.
    private const int RandomSeed = 778;
    private const int SmallestSize = 15;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BinarySearchFloodFill(),
            BuildHarness().BinarySearchFloodFill());

    [Fact]
    public void BinarySearchFloodFill_PermutationGrid_AgreesWithTheOtherArmAndTheJoiningRule()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapDijkstra(), harness.BinarySearchFloodFill());
        Assert.Equal(ExpectedMinimumTime(), harness.BinarySearchFloodFill());
    }

    [Fact]
    public void HeapDijkstra_PermutationGrid_AgreesWithTheOtherArmAndTheJoiningRule()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchFloodFill(), harness.HeapDijkstra());
        Assert.Equal(ExpectedMinimumTime(), harness.HeapDijkstra());
    }

    private static SwimInRisingWaterBenchmarks BuildHarness()
    {
        var harness = new SwimInRisingWaterBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }

    // Mirrors [GlobalSetup]'s own draw and layout.
    private static int[][] WorkloadGrid()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(0, SmallestSize * SmallestSize).OrderBy(_ => random.Next()).ToArray();
        var grid = new int[SmallestSize][];

        for (var row = 0; row < SmallestSize; row++)
        {
            grid[row] = new int[SmallestSize];

            for (var column = 0; column < SmallestSize; column++)
            {
                grid[row][column] = values[(row * SmallestSize) + column];
            }
        }

        return grid;
    }

    // The lowest elevation that joins the corners, tried in increasing order so the first one that
    // connects is the answer.
    private static int ExpectedMinimumTime()
    {
        var grid = WorkloadGrid();

        return Enumerable.Range(0, SmallestSize * SmallestSize)
            .First(elevation => JoinsCorners(grid, elevation));
    }

    private static bool JoinsCorners(int[][] grid, int elevation)
    {
        if (grid[0][0] > elevation || grid[SmallestSize - 1][SmallestSize - 1] > elevation)
        {
            return false;
        }

        var visited = new bool[SmallestSize, SmallestSize];
        var frontier = new Queue<(int Row, int Col)>();
        visited[0, 0] = true;
        frontier.Enqueue((0, 0));

        while (frontier.Count > 0)
        {
            var (row, column) = frontier.Dequeue();

            if (row == SmallestSize - 1 && column == SmallestSize - 1)
            {
                return true;
            }

            VisitIfLowEnough(grid, visited, frontier, elevation, row - 1, column);
            VisitIfLowEnough(grid, visited, frontier, elevation, row + 1, column);
            VisitIfLowEnough(grid, visited, frontier, elevation, row, column - 1);
            VisitIfLowEnough(grid, visited, frontier, elevation, row, column + 1);
        }

        return false;
    }

    private static void VisitIfLowEnough(
        int[][] grid, bool[,] visited, Queue<(int Row, int Col)> frontier, int elevation, int row, int column)
    {
        var isOffGrid = row < 0 || row >= SmallestSize || column < 0 || column >= SmallestSize;

        if (isOffGrid || visited[row, column] || grid[row][column] > elevation)
        {
            return;
        }

        visited[row, column] = true;
        frontier.Enqueue((row, column));
    }
}
