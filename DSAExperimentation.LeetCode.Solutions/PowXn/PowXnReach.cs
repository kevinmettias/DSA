namespace DSAExperimentation.LeetCode.Solutions.PowXn;

// The reach of PowXnSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them finishes
// in the harness's time. PowByRepeatedMultiplication multiplies |n| times, so its cost follows the power's
// magnitude whatever its sign; it takes 7.7 ms over the 72 of the catalog's 78 cases with |n| up to
// 1,000,001 (measured 2026-11-05, Release), and the cases near 2^31 run past the 250 ms a strategy's catalog
// cases are allowed. A bound on n itself would keep the most negative powers, the slowest of all.
internal static class PowXnReach
{
    private const long LargestPowerMagnitude = 1_000_001;

    public static bool IsWithin(double baseValue, int power) => Math.Abs((long)power) <= LargestPowerMagnitude;
}
