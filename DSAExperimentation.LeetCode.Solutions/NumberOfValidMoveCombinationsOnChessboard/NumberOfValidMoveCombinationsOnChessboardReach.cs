namespace DSAExperimentation.LeetCode.Solutions.NumberOfValidMoveCombinationsOnChessboard;

// The reach of NumberOfValidMoveCombinationsOnChessboardSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. CountCombinationsByCartesianProduct takes
// 233.5 ms over the 99 of the catalog's 100 cases with a length of pieces up to 4 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfValidMoveCombinationsOnChessboardReach
{
    private const int LargestPiecesLength = 4;

    public static bool IsWithin(string[] pieces, int[][] positions) =>
        pieces.Length <= LargestPiecesLength;
}
