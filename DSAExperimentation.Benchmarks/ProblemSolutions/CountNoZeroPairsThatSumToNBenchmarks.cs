using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNoZeroPairsThatSumToN;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNoZeroPairsThatSumToNSolution's, the same
// methods CountNoZeroPairsThatSumToNTests proves correct. TargetSum stays small
// enough for BruteForceSplit's O(n) scan to finish quickly - MemoizedDigitDp's
// whole point is that its own cost barely moves as TargetSum grows toward LC
// 3704's real 10^15 bound.
[MemoryDiagnoser]
public class CountNoZeroPairsThatSumToNBenchmarks
{
    [Params(100_000, 1_000_000)]
    public long TargetSum { get; set; }

    [Benchmark(Baseline = true)]
    public long BruteForceSplit() => CountNoZeroPairsThatSumToNSolution.CountPairsByBruteForce(TargetSum);

    [Benchmark]
    public long MemoizedDigitDp() => CountNoZeroPairsThatSumToNSolution.CountPairsByMemoizedDigitDp(TargetSum);
}
