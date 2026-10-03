namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 980: a rows x columns grid with the start (1),
// the end (2) and obstacleCount obstacles (-1) on distinct seeded cells and every
// other cell open (0) - exactly one start and one end, as LC 980 promises. Where
// the start and end land decides how many walks cover the grid, and so how much
// of the search tree survives to the end; placing them by seed rather than in
// opposite corners keeps that from being chosen by hand.
internal static class UniquePathsIIIWorkloads
{
    public const int Start = 1;
    public const int End = 2;
    public const int Obstacle = -1;

    // The start and the end take the first two seeded cells; obstacles take the ones after them.
    private const int EndpointCount = 2;

    public static int[][] BuildGrid(int rows, int columns, int obstacleCount, Random random)
    {
        var grid = Enumerable.Range(0, rows).Select(_ => new int[columns]).ToArray();
        var cells = SeededSequences.ShuffledZeroTo(rows * columns, random);

        Place(grid, cells[0], Start);
        Place(grid, cells[1], End);

        foreach (var cell in cells[EndpointCount..(EndpointCount + obstacleCount)])
        {
            Place(grid, cell, Obstacle);
        }

        return grid;
    }

    private static void Place(int[][] grid, int cell, int marker) =>
        grid[cell / grid[0].Length][cell % grid[0].Length] = marker;
}
