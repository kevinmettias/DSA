namespace DSAExperimentation.LeetCode.Solutions.MinimumOperationsToMakeArrayEqualToTarget;

// The reach of MinimumOperationsToMakeArrayEqualToTargetSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MinOperationsByBruteForceSimulation applies one unit
// of one operation per pass, so its time follows how far apart nums and target are, not their length: three
// elements 99,999,999 apart take it over a second, and a bound on length would keep one of the catalog's 105
// cases. The 97 cases where no element of nums is more than 999,999 from its target take it 8.4 ms (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumOperationsToMakeArrayEqualToTargetReach
{
    private const int LargestDifference = 999_999;

    public static bool IsWithin(int[] nums, int[] target)
    {
        for (var index = 0; index < nums.Length; index++)
        {
            if (Math.Abs(target[index] - nums[index]) > LargestDifference)
            {
                return false;
            }
        }

        return true;
    }
}
