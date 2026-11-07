namespace DSAExperimentation.LeetCode.Solutions.MaximumNumberOfMovesToKillAllPawns;

// The reach of MaximumNumberOfMovesToKillAllPawnsSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. The composed arm runs one BreadthFirstDistances search over
// the whole 50 x 50 board for the knight and for each pawn, so even a case of one pawn costs it 6 to 45 ms; over
// the catalog's 96 cases it takes 2.9 s, and the brute-force arm 1.3 s, most of it on the 15-pawn cases. Up to
// two pawns, the 4 cases it keeps take the composed arm 170 ms (measured 2026-11-06, Release, on the slower
// cores while another session's benchmark ran); up to three, 712 ms.
internal static class MaximumNumberOfMovesToKillAllPawnsReach
{
    private const int MostPawns = 2;

    public static bool IsWithin(int kx, int ky, int[][] positions) =>
        positions.Length <= MostPawns;
}
