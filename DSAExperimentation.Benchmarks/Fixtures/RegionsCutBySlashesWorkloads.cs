namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 959 - a square grid whose every cell is drawn
// uniformly from the three characters the problem allows, so roughly two thirds
// of the cells carry a cut and the region count stays well away from both
// degenerate ends (one region, or one region per triangle).
internal static class RegionsCutBySlashesWorkloads
{
    private static readonly char[] CellCharacters = [' ', '/', '\\'];

    public static string[] BuildGrid(int gridSize, int seed)
    {
        var random = new Random(seed);
        var grid = new string[gridSize];

        for (var r = 0; r < gridSize; r++)
        {
            var row = new char[gridSize];

            for (var c = 0; c < gridSize; c++)
            {
                row[c] = CellCharacters[random.Next(CellCharacters.Length)];
            }

            grid[r] = new string(row);
        }

        return grid;
    }
}
