using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LastRemainingIntegerAfterAlternatingDeletionOperations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LastRemainingIntegerAfterAlternatingDeletionOperationsSolution's, the same
// methods LastRemainingIntegerAfterAlternatingDeletionOperationsTests proves
// correct. N stays far below LC's own 10^15 upper bound - the O(n) list
// simulation would not finish otherwise; the head/step closed form is
// O(log n) regardless.
[MemoryDiagnoser]
public class LastRemainingIntegerAfterAlternatingDeletionOperationsBenchmarks
{
    [Params(10_000, 1_000_000)]
    public long N;

    [Benchmark(Baseline = true)]
    public long ListSimulation() =>
        LastRemainingIntegerAfterAlternatingDeletionOperationsSolution.FindLastRemainingByListSimulation(N);

    [Benchmark]
    public long HeadStepSimulation() =>
        LastRemainingIntegerAfterAlternatingDeletionOperationsSolution.FindLastRemainingByHeadStepSimulation(N);
}
