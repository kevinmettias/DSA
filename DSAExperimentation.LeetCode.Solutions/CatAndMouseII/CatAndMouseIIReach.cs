namespace DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

// The reach of CatAndMouseIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanMouseWinByExhaustiveRecursion takes 47.5 ms over the 2 of the catalog's
// 57 cases with a length of grid up to 1 (measured 2026-11-05, Release); the larger cases run past the 250 ms
// a strategy's catalog cases are allowed.
internal static class CatAndMouseIIReach
{
    private const int LargestGridLength = 1;

    public static bool IsWithin(string[] grid, int catJump, int mouseJump) =>
        grid.Length <= LargestGridLength;
}
