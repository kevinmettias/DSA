using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumIceCreamBars;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumIceCreamBarsSolution's, the same methods
// MaximumIceCreamBarsTests proves correct - an O(n^2) repeated-selection scan
// against one MergeSort followed by a single greedy pass. _coins is sized to roughly
// a quarter of the bars' total cost so both strategies are forced to scan well past
// the cheapest few bars instead of exiting after one or two purchases. The cost
// array is LeetCode's own input shape, so neither arm needs a hoisted overload.
[MemoryDiagnoser]
public class MaximumIceCreamBarsBenchmarks
{
    private const int MaxCostExclusive = 100;
    private const int CoinsPerBarDivisor = 4;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1833;

    [Params(200, 5_000)]
    public int Length;

    private int[] _costs = null!;
    private int _coins;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _costs = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCostExclusive)).ToArray();
        _coins = (Length * MaxCostExclusive) / CoinsPerBarDivisor;
    }

    [Benchmark(Baseline = true)]
    public int SelectionScan() => MaximumIceCreamBarsSolution.MaxIceCreamBySelectionScan(_costs, _coins);

    [Benchmark]
    public int MergeSortGreedy() => MaximumIceCreamBarsSolution.MaxIceCreamByMergeSortGreedy(_costs, _coins);
}
