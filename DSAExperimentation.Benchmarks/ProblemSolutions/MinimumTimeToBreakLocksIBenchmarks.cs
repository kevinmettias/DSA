using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToBreakLocksI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToBreakLocksISolution's, the same
// methods MinimumTimeToBreakLocksITests proves correct. LockCount is capped
// at 8 - LC 3376's own constraint - so the permutation baseline's n! blowup
// is still measurable rather than practically infinite.
[MemoryDiagnoser]
public class MinimumTimeToBreakLocksIBenchmarks
{
    private const int Seed = 3376;
    private const int MaxStrengthExclusive = 1_000_000;
    private const int K = 3;

    [Params(4, 8)]
    public int LockCount;

    private int[] _strength = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _strength = Enumerable.Range(0, LockCount).Select(_ => random.Next(1, MaxStrengthExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PermutationBruteForce() =>
        MinimumTimeToBreakLocksISolution.FindMinimumTimeByPermutationBruteForce(_strength, K);

    [Benchmark]
    public int BitmaskMemo() =>
        MinimumTimeToBreakLocksISolution.FindMinimumTimeByBitmaskMemo(_strength, K);
}
