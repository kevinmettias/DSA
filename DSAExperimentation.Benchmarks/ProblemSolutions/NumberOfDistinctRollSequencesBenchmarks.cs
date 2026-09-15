using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfDistinctRollSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfDistinctRollSequencesSolution's, the same
// methods NumberOfDistinctRollSequencesTests proves correct - the unmemoized
// recursion over (day, secondLastRoll, lastRoll), which re-explores every state on
// each path that reaches it, against the same recurrence routed through this repo's
// own Memoizer. N stays small enough that the exponential arm still finishes
// (CanIWinBenchmarks' precedent for bounding a brute-force baseline's input size).
[MemoryDiagnoser]
public class NumberOfDistinctRollSequencesBenchmarks
{
    [Params(6, 10)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public long BruteForceRecursion() =>
        NumberOfDistinctRollSequencesSolution.DistinctSequencesByBruteForceRecursion(N);

    [Benchmark]
    public long MemoizedRecursion() =>
        NumberOfDistinctRollSequencesSolution.DistinctSequencesByMemoizedRecursion(N);
}
