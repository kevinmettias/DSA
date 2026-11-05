namespace DSAExperimentation.LeetCode.Solutions.PartitionToKEqualSumSubsets;

// The reach of PartitionToKEqualSumSubsetsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CanPartitionKSubsetsByGenericBacktrack takes 8.7 ms over the 8
// of the catalog's 85 cases with a length of nums up to 15 and a value of bucketCount up to 4 (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class PartitionToKEqualSumSubsetsReach
{
    private const int LargestNumsLength = 15;
    private const int LargestBucketCount = 4;

    public static bool IsWithin(int[] nums, int bucketCount) =>
        nums.Length <= LargestNumsLength && bucketCount <= LargestBucketCount;
}
