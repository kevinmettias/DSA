namespace DSAExperimentation.LeetCode.Solutions.FindTheLargestPalindromeDivisibleByK;

// The reach of FindTheLargestPalindromeDivisibleByKSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. LargestPalindromeByBruteForce takes 157.8 ms over
// the 41 of the catalog's 49 cases with a value of digitCount up to 14 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindTheLargestPalindromeDivisibleByKReach
{
    private const int LargestDigitCount = 14;

    public static bool IsWithin(int digitCount, int divisor) =>
        digitCount <= LargestDigitCount;
}
