namespace DSAExperimentation.LeetCode.Solutions.LongestIncreasingPathInAMatrix;

// The reach of LongestIncreasingPathInAMatrixSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. LongestPathByNaiveRecursion walks every increasing path
// from every cell again, and over the 80 of the catalog's 84 cases with up to 50 cells neither arm takes more
// than 15.6 ms (measured 2026-11-05, Release); past them a 9 x 9 matrix takes the naive arm a second, and a
// 10 x 10 snake of rising values runs it past 1.5 s.
internal static class LongestIncreasingPathInAMatrixReach
{
    private const int LargestCellCount = 50;

    public static bool IsWithin(int[,] matrix) =>
        matrix.Length <= LargestCellCount;
}
