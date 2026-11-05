namespace DSAExperimentation.LeetCode.Solutions.FindTheMinimumCostArrayPermutation;

// The reach of FindTheMinimumCostArrayPermutationSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. FindPermutationByBruteForceSearch takes 234.7 ms
// over the 8 of the catalog's 111 cases with a length of nums up to 10 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindTheMinimumCostArrayPermutationReach
{
    private const int LargestNumsLength = 10;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
