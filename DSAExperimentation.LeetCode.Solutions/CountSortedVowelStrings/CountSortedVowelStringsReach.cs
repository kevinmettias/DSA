namespace DSAExperimentation.LeetCode.Solutions.CountSortedVowelStrings;

// The reach of CountSortedVowelStringsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. CountVowelStringsByBacktrackingEnumeration takes 204.1 ms over the
// 23 of the catalog's 24 cases with a value of length up to 49 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CountSortedVowelStringsReach
{
    private const int LargestLength = 49;

    public static bool IsWithin(int length) =>
        length <= LargestLength;
}
