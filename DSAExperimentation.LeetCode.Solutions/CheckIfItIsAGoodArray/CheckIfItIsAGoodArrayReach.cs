namespace DSAExperimentation.LeetCode.Solutions.CheckIfItIsAGoodArray;

// The reach of CheckIfItIsAGoodArraySolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. IsGoodArrayBySubtractionGcd spends one subtraction per multiple of the
// smaller number in the larger, so its time follows the numbers' size, not their count, and a bound on length
// would keep only 11 of the catalog's 122 cases. Sorted by the sum of their elements instead, the 113 cases up
// to a sum of 1,937,500,000 take it 183.8 ms (measured 2026-11-05, Release); the larger cases run past the
// 250 ms a strategy's catalog cases are allowed. The sum is a measured cut over the catalog's cases, not the
// arm's cost: two numbers far apart in size cost more than their sum says.
internal static class CheckIfItIsAGoodArrayReach
{
    private const long LargestElementSum = 1_937_500_000;

    public static bool IsWithin(int[] nums) =>
        nums.Sum(number => (long)number) <= LargestElementSum;
}
