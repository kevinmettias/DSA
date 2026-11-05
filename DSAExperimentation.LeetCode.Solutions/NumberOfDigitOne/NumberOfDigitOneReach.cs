namespace DSAExperimentation.LeetCode.Solutions.NumberOfDigitOne;

// The reach of NumberOfDigitOneSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CountDigitOneByBruteForceScan takes 140.3 ms over the 36 of the
// catalog's 67 cases with a value of upperBound up to 10,000,000 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfDigitOneReach
{
    private const int LargestUpperBound = 10_000_000;

    public static bool IsWithin(int upperBound) =>
        upperBound <= LargestUpperBound;
}
