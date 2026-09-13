using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.New21Game;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are New21GameSolution's, the same methods New21GameTests
// proves correct. MaxPts is fixed at 6 and K is the varying [Params] axis (N == K,
// so only the reachability of the recursion tree matters, not the final probability
// value) so both the recursion depth (bounded by K) and the branching factor
// (MaxPts) stay large enough for the un-memoized tree's overlapping-state blowup to
// show clearly without becoming impractically slow.
//
// There is no [GlobalSetup] left to charge: the only preparation the old harness did
// was copying K into an N field, and N is passed straight through to both arms.
[MemoryDiagnoser]
public class New21GameBenchmarks
{
    private const int MaxPts = 6;

    [Params(12, 20)]
    public int K;

    [Benchmark(Baseline = true)]
    public double UnmemoizedRecursion() =>
        New21GameSolution.ProbabilityByUnmemoizedRecursion(K, K, MaxPts);

    [Benchmark]
    public double MemoizedRecursion() =>
        New21GameSolution.ProbabilityByMemoizedRecursion(K, K, MaxPts);
}
