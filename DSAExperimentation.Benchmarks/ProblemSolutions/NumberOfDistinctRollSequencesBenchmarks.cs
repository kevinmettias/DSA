using DSAExperimentation.LeetCode.NumberOfDistinctRollSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfDistinctRollSequencesSolution's, the same
// methods NumberOfDistinctRollSequencesSolutionTests proves correct - the unmemoized
// recursion over (day, secondLastRoll, lastRoll), which re-explores every state on
// each path that reaches it, against the same recurrence routed through this repo's
// own Memoizer.
//
// Sizes are per arm. The exponential arm stops at 10 rolls (CanIWinBenchmarks'
// precedent for bounding a brute-force baseline's input size); the memoized arm's
// O(n) states run on to 1,000, short of LC 2318's 10^4 because Memoizer recurses once
// per day and a deeper chain would put the call stack at risk. The two are compared at
// the sizes both run.
public class NumberOfDistinctRollSequencesBenchmarks
{
    public static IEnumerable<int> BaselineSizes => [6, 10];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public long BruteForceRecursion(int sequenceLength) =>
        NumberOfDistinctRollSequencesSolution.DistinctSequencesByBruteForceRecursion(sequenceLength);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public long MemoizedRecursion(int sequenceLength) =>
        NumberOfDistinctRollSequencesSolution.DistinctSequencesByMemoizedRecursion(sequenceLength);
}
