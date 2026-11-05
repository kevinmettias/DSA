namespace DSAExperimentation.LeetCode.Solutions.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

// The reach of PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution's strategies (ARCHITECTURE 17.13):
// the catalog cases every one of them finishes in the harness's time.
// MinimumDifferenceByBruteForceEqualSplits takes 187.7 ms over the 73 of the catalog's 77 cases with a length
// of nums up to 18 (measured 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog
// cases are allowed.
internal static class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceReach
{
    private const int LargestNumsLength = 18;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
