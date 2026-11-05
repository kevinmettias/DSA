namespace DSAExperimentation.LeetCode.Solutions.BeautifulArrangement;

// The reach of BeautifulArrangementSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CountByGenerateThenFilter takes 86.7 ms over the 9 of the catalog's 15
// cases with a value of size up to 9 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class BeautifulArrangementReach
{
    private const int LargestSize = 9;

    public static bool IsWithin(int size) =>
        size <= LargestSize;
}
