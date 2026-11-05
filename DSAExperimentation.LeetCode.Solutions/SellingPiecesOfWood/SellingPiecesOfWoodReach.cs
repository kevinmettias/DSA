namespace DSAExperimentation.LeetCode.Solutions.SellingPiecesOfWood;

// The reach of SellingPiecesOfWoodSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. SellingWoodByUnmemoizedRecursion takes 55 ms over the 8 of the
// catalog's 88 cases with a value of boardHeight up to 5 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class SellingPiecesOfWoodReach
{
    private const int LargestBoardHeight = 5;

    public static bool IsWithin(int boardHeight, int boardWidth, ValueTuple<int, int, int>[] prices) =>
        boardHeight <= LargestBoardHeight;
}
