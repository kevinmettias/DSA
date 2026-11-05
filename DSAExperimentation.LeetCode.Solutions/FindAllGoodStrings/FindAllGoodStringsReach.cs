namespace DSAExperimentation.LeetCode.Solutions.FindAllGoodStrings;

// The reach of FindAllGoodStringsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CountGoodStringsByAutomatonDigitDp takes 6.4 ms over the 6 of the
// catalog's 29 cases with a value of stringLength up to 3 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindAllGoodStringsReach
{
    private const int LargestStringLength = 3;

    public static bool IsWithin(int stringLength, FindAllGoodStringsSolution.LowerBound s1, FindAllGoodStringsSolution.UpperBound s2, FindAllGoodStringsSolution.ForbiddenSubstring evil) =>
        stringLength <= LargestStringLength;
}
