namespace DSAExperimentation.LeetCode.Solutions.PalindromePartitioningIII;

// The reach of PalindromePartitioningIIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. MinChangesByNaiveRecursion takes 22.3 ms over the 86 of the
// catalog's 89 cases with a length of text up to 27 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class PalindromePartitioningIIIReach
{
    private const int LargestTextLength = 27;

    public static bool IsWithin(string text, int partitionCount) =>
        text.Length <= LargestTextLength;
}
