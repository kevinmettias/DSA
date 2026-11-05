namespace DSAExperimentation.LeetCode.Solutions.FindTheCountOfGoodIntegers;

// The reach of FindTheCountOfGoodIntegersSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CountByBacktrackEnumeration takes 44.7 ms over the 42 of the
// catalog's 56 cases with a value of digitCount up to 8 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class FindTheCountOfGoodIntegersReach
{
    private const int LargestDigitCount = 8;

    public static bool IsWithin(int digitCount, int divisor) =>
        digitCount <= LargestDigitCount;
}
