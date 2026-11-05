namespace DSAExperimentation.LeetCode.Solutions.VowelsGameInAString;

// The reach of VowelsGameInAStringSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CanAliceWinByGameSearch takes 238.3 ms over the 82 of the catalog's
// 120 cases with a length of text up to 36 (measured 2026-11-05, Release); the larger cases run past the 250
// ms a strategy's catalog cases are allowed.
internal static class VowelsGameInAStringReach
{
    private const int LargestTextLength = 36;

    public static bool IsWithin(string text) =>
        text.Length <= LargestTextLength;
}
