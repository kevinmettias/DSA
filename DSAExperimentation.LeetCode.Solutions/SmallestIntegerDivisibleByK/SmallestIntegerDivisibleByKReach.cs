namespace DSAExperimentation.LeetCode.Solutions.SmallestIntegerDivisibleByK;

// The reach of SmallestIntegerDivisibleByKSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. SmallestRepunitLengthByReduceGraph takes 211.3 ms over the 57
// of the catalog's 70 cases with a value of divisor up to 75,321 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class SmallestIntegerDivisibleByKReach
{
    private const int LargestDivisor = 75_321;

    public static bool IsWithin(int divisor) =>
        divisor <= LargestDivisor;
}
