namespace DSAExperimentation.LeetCode.Solutions.MaximumAndSumOfArray;

// The reach of MaximumAndSumOfArraySolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. MaximumAndSumByBruteForceRecursion takes 157.2 ms over the 19 of the
// catalog's 72 cases with a length of nums up to 9 and a value of numSlots up to 7 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MaximumAndSumOfArrayReach
{
    private const int LargestNumsLength = 9;
    private const int LargestNumSlots = 7;

    public static bool IsWithin(int[] nums, int numSlots) =>
        nums.Length <= LargestNumsLength && numSlots <= LargestNumSlots;
}
