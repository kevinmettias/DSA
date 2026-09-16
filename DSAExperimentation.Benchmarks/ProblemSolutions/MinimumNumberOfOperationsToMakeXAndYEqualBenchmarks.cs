using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeXAndYEqual;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfOperationsToMakeXAndYEqualSolution's,
// the same methods MinimumNumberOfOperationsToMakeXAndYEqualTests proves correct
// (TwoSumBenchmarks precedent). StartValue is deliberately far from a multiple of 5
// or 11 (and from Y) so the mutation queue has to explore a wide swath of the bounded
// range before it stumbles onto a divide, while the memoized recurrence only ever
// follows the O(log StartValue) chain of "round to a multiple, then divide" states.
[MemoryDiagnoser]
public class MinimumNumberOfOperationsToMakeXAndYEqualBenchmarks
{
    private const int Y = 1;

    [Params(997, 9_973)]
    public int StartValue { get; set; }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        MinimumNumberOfOperationsToMakeXAndYEqualSolution.MinOperationsByMutationQueue(StartValue, Y);

    [Benchmark]
    public int MemoizedReduce() =>
        MinimumNumberOfOperationsToMakeXAndYEqualSolution.MinOperationsByMemoizedReduce(StartValue, Y);
}
