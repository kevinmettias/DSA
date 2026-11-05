namespace DSAExperimentation.LeetCode.Solutions.SplitArrayWithSameAverage;

// The reach of SplitArrayWithSameAverageSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CanSplitBySubsetMasks takes 26.5 ms over the 39 of the
// catalog's 87 cases with a length of nums up to 19 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class SplitArrayWithSameAverageReach
{
    private const int LargestNumsLength = 19;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
