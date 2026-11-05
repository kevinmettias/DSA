namespace DSAExperimentation.LeetCode.Solutions.ClosestDivisors;

// The reach of ClosestDivisorsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. ClosestPairByDivisorScan takes 194 ms over the 21 of the catalog's 49 cases
// with a value of num up to 42,424,242 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class ClosestDivisorsReach
{
    private const int LargestNum = 42_424_242;

    public static bool IsWithin(int num) =>
        num <= LargestNum;
}
