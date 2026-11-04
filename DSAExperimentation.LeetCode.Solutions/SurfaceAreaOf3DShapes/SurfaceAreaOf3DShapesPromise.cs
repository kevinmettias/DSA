namespace DSAExperimentation.LeetCode.Solutions.SurfaceAreaOf3DShapes;

// LC 892's promise about its input: the grid is n by n. LeetCode's judge never poses an input that
// breaks it. A row shorter or longer than the grid is tall is read past an end. Generated cases do
// not always keep the promise, so a harness that runs them asks this first.
internal static class SurfaceAreaOf3DShapesPromise
{
    public static bool IsKeptBy(int[][] grid) => grid.All(row => row.Length == grid.Length);
}
