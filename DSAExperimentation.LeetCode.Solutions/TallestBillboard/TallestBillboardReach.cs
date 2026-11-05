namespace DSAExperimentation.LeetCode.Solutions.TallestBillboard;

// The reach of TallestBillboardSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. MaxHeightByMemoizedDiff takes 4.5 ms over the 21 of the catalog's 72
// cases with a length of rods up to 9 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class TallestBillboardReach
{
    private const int LargestRodsLength = 9;

    public static bool IsWithin(int[] rods) =>
        rods.Length <= LargestRodsLength;
}
