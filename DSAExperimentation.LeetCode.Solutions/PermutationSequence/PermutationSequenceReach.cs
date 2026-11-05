namespace DSAExperimentation.LeetCode.Solutions.PermutationSequence;

// The reach of PermutationSequenceSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. GetPermutationByBacktrackEnumeration takes 166.7 ms over the 58 of the
// catalog's 58 cases with a value of digitCount up to 9 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class PermutationSequenceReach
{
    private const int LargestDigitCount = 9;

    public static bool IsWithin(int digitCount, int rank) =>
        digitCount <= LargestDigitCount;
}
