namespace DSAExperimentation.LeetCode.Solutions.SpecialPermutations;

// The reach of SpecialPermutationsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CountByBruteForceBacktracking takes 30.7 ms over the 35 of the
// catalog's 76 cases with a length of nums up to 8 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class SpecialPermutationsReach
{
    private const int LargestNumsLength = 8;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
