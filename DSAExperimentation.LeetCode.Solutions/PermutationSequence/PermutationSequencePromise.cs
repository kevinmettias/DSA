namespace DSAExperimentation.LeetCode.Solutions.PermutationSequence;

// LC 60's promise about its input: "1 <= k <= n!", so the rank names one of the n! permutations of 1 to n.
// LeetCode's judge never poses an input that breaks it. Past the last permutation both strategies run out
// of permutations and answer the empty string. Generated cases do not always keep the promise (one asks
// for the 40,321st of 8! = 40,320), so a harness that runs them asks this first.
internal static class PermutationSequencePromise
{
    private const int FirstRank = 1;

    public static bool IsKeptBy(int digitCount, int rank) =>
        rank >= FirstRank && rank <= Enumerable.Range(1, digitCount).Aggregate(1L, (product, factor) => product * factor);
}
