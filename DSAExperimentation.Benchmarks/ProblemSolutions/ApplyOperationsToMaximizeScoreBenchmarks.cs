using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ApplyOperationsToMaximizeScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ApplyOperationsToMaximizeScoreSolution's, the same
// methods ApplyOperationsToMaximizeScoreTests proves correct. They share the prime
// score precompute and the greedy modular-power spend, so what separates them is how
// each index finds its two boundaries: an outward scan that is O(n) per index whenever
// scores run long ties - common here, since a prime score only ranges over a handful
// of small integers - against one monotonic pass per side over this repo's own
// Stack<int>, O(n) overall. int[] plus an int budget is already LeetCode's own input
// shape, so [GlobalSetup] hands it straight in and no hoisted overload is needed.
[MemoryDiagnoser]
public class ApplyOperationsToMaximizeScoreBenchmarks
{
    private const int RandomSeed = 2818; // LeetCode problem number
    private const int MaxValueExclusive = 100_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(2, MaxValueExclusive)).ToArray();
        _k = Length;
    }

    [Benchmark(Baseline = true)]
    public long LinearBoundaryScan() =>
        ApplyOperationsToMaximizeScoreSolution.MaximumScoreByLinearBoundaryScan(_nums, _k);

    [Benchmark]
    public long StackBoundaryScan() =>
        ApplyOperationsToMaximizeScoreSolution.MaximumScoreByStackBoundaryScan(_nums, _k);
}
