using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LeastOperatorsToExpressNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LeastOperatorsToExpressNumberSolution's, the same
// methods LeastOperatorsToExpressNumberTests proves correct. Base 2 makes the
// base-digit recursion as deep and as branch-heavy as it gets, so the
// un-memoized arm re-explores the same (remaining, level) pairs from many root
// paths while the memoized arm collapses them. TargetBitLength is kept modest
// specifically because that blowup is real, the same reasoning
// BurstBalloonsBenchmarks' BalloonCount cap already documents. Target construction
// is charged to [GlobalSetup].
[MemoryDiagnoser]
public class LeastOperatorsToExpressNumberBenchmarks
{
    private const int X = 2;

    private int _target;

    [Params(16, 20)]
    public int TargetBitLength { get; set; }

    [GlobalSetup]
    public void Setup() => _target = (1 << TargetBitLength) - 1;

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByUnmemoizedRecursion(X, _target);

    [Benchmark]
    public int MemoizedRecursion() =>
        LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByMemoizedRecursion(X, _target);
}
