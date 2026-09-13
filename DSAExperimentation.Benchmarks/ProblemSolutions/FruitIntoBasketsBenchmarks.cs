using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FruitIntoBaskets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FruitIntoBasketsSolution's, the same methods
// FruitIntoBasketsTests proves correct. _fruits alternates between only 2 tree
// types so the window (and the per-start rescan's inner scan) never needs to shrink
// for a third type - forcing BOTH strategies through their full-length scan instead
// of the rescan breaking out after only 3 elements every time, the same "force the
// real worst case" convention TwoSumBenchmarks/SubarrayProductLessThanKBenchmarks
// establish.
[MemoryDiagnoser]
public class FruitIntoBasketsBenchmarks
{
    private const int TreeTypes = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _fruits = null!;

    [GlobalSetup]
    public void Setup() => _fruits = Enumerable.Range(0, Length).Select(i => i % TreeTypes).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() => FruitIntoBasketsSolution.TotalFruitByPerStartRescan(_fruits);

    [Benchmark]
    public int SlidingWindowHashMap() => FruitIntoBasketsSolution.TotalFruitBySlidingWindow(_fruits);
}
