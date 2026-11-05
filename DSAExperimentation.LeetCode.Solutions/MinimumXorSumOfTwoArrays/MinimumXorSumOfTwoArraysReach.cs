namespace DSAExperimentation.LeetCode.Solutions.MinimumXorSumOfTwoArrays;

// The reach of MinimumXorSumOfTwoArraysSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. MinimumXorSumByMemoizedBitmask takes 4.3 ms over the 24 of the
// catalog's 90 cases with a length of nums1 up to 4 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumXorSumOfTwoArraysReach
{
    private const int LargestNums1Length = 4;

    public static bool IsWithin(int[] nums1, int[] nums2) =>
        nums1.Length <= LargestNums1Length;
}
