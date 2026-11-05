namespace DSAExperimentation.LeetCode.Solutions.DigitOperationsToMakeTwoIntegersEqual;

// The reach of DigitOperationsToMakeTwoIntegersEqualSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MinOperationsByDijkstraOverDigitGraph takes 234.4
// ms over the 84 of the catalog's 111 cases with a value of startValue up to 1,024 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class DigitOperationsToMakeTwoIntegersEqualReach
{
    private const int LargestStartValue = 1024;

    public static bool IsWithin(int startValue, int targetValue) =>
        startValue <= LargestStartValue;
}
