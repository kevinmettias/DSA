using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfPointIsReachable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfPointIsReachableSolution's, the same methods
// CheckIfPointIsReachableTests proves correct. The contrast is "materialize and
// search the whole state space vs. one closed-form check", the same shape
// TwoSumBenchmarks runs for its brute-force-vs-hash-map pair. Targets are
// consecutive integers (gcd 1, always reachable) so the search is always forced to
// explore rather than short-circuiting on an early false.
[MemoryDiagnoser]
public class CheckIfPointIsReachableBenchmarks
{
    private int _targetX;

    private int _targetY;
    [Params(50, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _targetX = Length;
        _targetY = Length + 1;
    }

    [Benchmark(Baseline = true)]
    public bool IsReachableByBruteForceBfs() =>
        CheckIfPointIsReachableSolution.IsReachableByBruteForceBfs(_targetX, _targetY);

    [Benchmark]
    public bool IsReachableByGcd() =>
        CheckIfPointIsReachableSolution.IsReachableByGcd(_targetX, _targetY);
}
