namespace DSAExperimentation.LeetCode.Solutions.RegionsCutBySlashes;

// LC 959's promise about its input: the grid is n by n. LeetCode's judge never poses an input that
// breaks it. A row shorter or longer than the grid is tall is read past an end. Generated cases do
// not always keep the promise, so a harness that runs them asks this first.
internal static class RegionsCutBySlashesPromise
{
    public static bool IsKeptBy(string[] grid) => grid.All(row => row.Length == grid.Length);
}
