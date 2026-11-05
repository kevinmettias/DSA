namespace DSAExperimentation.LeetCode.Solutions.MostFrequentPrime;

// The reach of MostFrequentPrimeSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. MostFrequentPrimeBySieve takes 150.7 ms over the 54 of the catalog's
// 60 cases with an element count of mat up to 36 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class MostFrequentPrimeReach
{
    private const int LargestMatElements = 36;

    public static bool IsWithin(int[][] mat) =>
        mat.Sum(row => row.Length) <= LargestMatElements;
}
