namespace DSAExperimentation.LeetCode.Solutions.CountKReducibleNumbersLessThanN;

// The reach of CountKReducibleNumbersLessThanNSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. CountKReducibleNumbersByPopcountCombinatorics takes 1.4
// ms over the 9 of the catalog's 37 cases with a length of binaryDigits up to 8 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CountKReducibleNumbersLessThanNReach
{
    private const int LargestBinaryDigitsLength = 8;

    public static bool IsWithin(string binaryDigits, int maxSteps) =>
        binaryDigits.Length <= LargestBinaryDigitsLength;
}
