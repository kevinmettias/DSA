namespace DSAExperimentation.LeetCode.Solutions.MinimumMovesToSpreadStonesOverGrid;

// LC 2850's promise about its input: the grid is three by three and holds nine stones, one for each
// cell. LeetCode's judge never poses an input that breaks it. With more stones than cells, the
// permutations pair a surplus with no empty cell. Generated cases do not always keep the promise,
// so a harness that runs them asks this first.
internal static class MinimumMovesToSpreadStonesOverGridPromise
{
    public static bool IsKeptBy(int[][] grid) => HasOneStonePerCell(grid, MinimumMovesToSpreadStonesOverGridSolution.GridSize);

    private static bool HasOneStonePerCell(int[][] grid, int side) => IsSquare(grid, side) && grid.Sum(row => row.Sum()) == side * side;

    private static bool IsSquare(int[][] grid, int side) => grid.Length == side && grid.All(row => row.Length == side);
}
