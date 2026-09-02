using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.JumpGameIX;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameIXSolution's, the same methods
// JumpGameIXTests proves correct. nums is built once in [GlobalSetup]; both arms
// take LeetCode's own array shape directly, so there is nothing further to hoist.
// NumCount stays small - the baseline's per-index BFS re-scans every other index at
// every step, O(n^3) worst case.
[MemoryDiagnoser]
public class JumpGameIXBenchmarks
{
    private const int Seed = 3660;

    [Params(20, 80)]
    public int NumCount;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = JumpGameIXWorkloads.Build(NumCount, seed: Seed);

    [Benchmark(Baseline = true)]
    public int[] JumpBfs() => JumpGameIXSolution.MaxValuesByJumpBfs(_nums);

    [Benchmark]
    public int[] AdjacentUnionFind() => JumpGameIXSolution.MaxValuesByAdjacentUnionFind(_nums);
}
