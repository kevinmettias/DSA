namespace DSAExperimentation.LeetCode.Solutions.ManhattanDistancesOfAllArrangementsOfPieces;

// The reach of ManhattanDistancesOfAllArrangementsOfPiecesSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. SumByPairwiseDistanceFormula takes 1.1 ms
// over the 3 of the catalog's 89 cases with a value of pieceCount up to 4 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class ManhattanDistancesOfAllArrangementsOfPiecesReach
{
    private const int LargestPieceCount = 4;

    public static bool IsWithin(int rowCount, int columnCount, int pieceCount) =>
        pieceCount <= LargestPieceCount;
}
