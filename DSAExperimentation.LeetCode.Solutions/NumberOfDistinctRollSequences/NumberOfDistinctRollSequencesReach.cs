namespace DSAExperimentation.LeetCode.Solutions.NumberOfDistinctRollSequences;

// The reach of NumberOfDistinctRollSequencesSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. DistinctSequencesByBruteForceRecursion takes 189.8 ms
// over the 9 of the catalog's 20 cases with a value of sequenceLength up to 15 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfDistinctRollSequencesReach
{
    private const int LargestSequenceLength = 15;

    public static bool IsWithin(int sequenceLength) =>
        sequenceLength <= LargestSequenceLength;
}
