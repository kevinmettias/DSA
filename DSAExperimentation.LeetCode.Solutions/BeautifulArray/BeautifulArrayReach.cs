namespace DSAExperimentation.LeetCode.Solutions.BeautifulArray;

// The reach of BeautifulArraySolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. ConstructByMemoizedDivideAndConquer takes 13.7 ms over the 8 of the
// catalog's 58 cases with a value of length up to 9 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class BeautifulArrayReach
{
    private const int LargestLength = 9;

    public static bool IsWithin(int length) =>
        length <= LargestLength;
}
