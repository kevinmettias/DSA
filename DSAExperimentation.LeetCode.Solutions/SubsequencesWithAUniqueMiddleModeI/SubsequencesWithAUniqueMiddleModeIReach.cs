namespace DSAExperimentation.LeetCode.Solutions.SubsequencesWithAUniqueMiddleModeI;

// The reach of SubsequencesWithAUniqueMiddleModeISolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. CountMiddleModeSubsequencesByBruteForce takes 220
// ms over the 44 of the catalog's 133 cases with a length of nums up to 28 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class SubsequencesWithAUniqueMiddleModeIReach
{
    private const int LargestNumsLength = 28;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
