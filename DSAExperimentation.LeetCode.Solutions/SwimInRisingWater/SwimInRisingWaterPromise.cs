namespace DSAExperimentation.LeetCode.Solutions.SwimInRisingWater;

// LC 778's promise about its input: the grid is n by n and holds each of 0 to n^2 - 1 once.
// LeetCode's judge never poses an input that breaks it. Repeated elevations tie, and the flood fill
// and the heap break the tie differently. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class SwimInRisingWaterPromise
{
    public static bool IsKeptBy(int[][] grid)
    {
        var everyElevation = Enumerable.Range(0, grid.Length * grid.Length);

        return grid.All(row => row.Length == grid.Length) && grid.SelectMany(row => row).Order().SequenceEqual(everyElevation);
    }
}
