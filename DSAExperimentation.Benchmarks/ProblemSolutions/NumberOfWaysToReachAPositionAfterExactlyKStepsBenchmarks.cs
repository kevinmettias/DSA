using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysToReachAPositionAfterExactlyKSteps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfWaysToReachAPositionAfterExactlyKStepsSolution's, the same strategies
// NumberOfWaysToReachAPositionAfterExactlyKStepsTests proves correct. The endpoints
// are two apart, preserving the pre-migration workload's distance, and K stays
// modest specifically because the un-memoized baseline's 2^K blowup is real - the
// same reasoning TargetSumBenchmarks and FibonacciNumberBenchmarks document.
[MemoryDiagnoser]
public class NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarks
{
    private const int StartPos = 0;
    private const int EndPos = 2;

    [Params(18, 22)]
    public int K { get; set; }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByUnmemoizedRecursion(
            StartPos, EndPos, K);

    [Benchmark]
    public int MemoizedRecursion() =>
        NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByMemoizedRecursion(
            StartPos, EndPos, K);
}
