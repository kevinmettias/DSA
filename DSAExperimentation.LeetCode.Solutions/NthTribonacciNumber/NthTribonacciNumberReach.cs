namespace DSAExperimentation.LeetCode.Solutions.NthTribonacciNumber;

// The reach of NthTribonacciNumberSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. TribonacciByNaiveRecursion takes 114.2 ms over the 16 of the catalog's
// 18 cases with a value of termIndex up to 30 (measured 2026-11-05, Release); the larger cases run past the
// 250 ms a strategy's catalog cases are allowed.
internal static class NthTribonacciNumberReach
{
    private const int LargestTermIndex = 30;

    public static bool IsWithin(int termIndex) =>
        termIndex <= LargestTermIndex;
}
