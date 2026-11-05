namespace DSAExperimentation.LeetCode.Solutions.PrimePalindrome;

// The reach of PrimePalindromeSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. SmallestPrimePalindromeBySequentialScan takes 48.1 ms over the 28 of the
// catalog's 44 cases with a value of lowerBound up to 9,988,999 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class PrimePalindromeReach
{
    private const int LargestLowerBound = 9_988_999;

    public static bool IsWithin(int lowerBound) =>
        lowerBound <= LargestLowerBound;
}
