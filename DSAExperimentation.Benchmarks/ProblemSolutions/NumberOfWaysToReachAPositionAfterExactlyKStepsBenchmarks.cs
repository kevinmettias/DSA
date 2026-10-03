using DSAExperimentation.LeetCode.NumberOfWaysToReachAPositionAfterExactlyKSteps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfWaysToReachAPositionAfterExactlyKStepsSolution's, the same strategies
// NumberOfWaysToReachAPositionAfterExactlyKStepsSolutionTests proves correct. The endpoints
// are two apart, preserving the pre-migration workload's distance, and start at 1, the
// lowest position LC 2400 allows. StepCount stays modest specifically because the
// un-memoized baseline's 2^StepCount blowup is real - the same reasoning
// TargetSumBenchmarks and FibonacciNumberBenchmarks document.
public class NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarks
{
    private const int StartPos = 1;
    private const int EndPos = 3;

    [Params(18, 22)]
    public int StepCount { get; set; }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByUnmemoizedRecursion(
            StartPos, EndPos, StepCount);

    [Benchmark]
    public int MemoizedRecursion() =>
        NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByMemoizedRecursion(
            StartPos, EndPos, StepCount);
}
