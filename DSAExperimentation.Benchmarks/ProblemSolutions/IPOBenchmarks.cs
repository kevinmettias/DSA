using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IPO;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IPOSolution's, the same methods IPOTests proves
// correct - the O(k*n) linear-rescan baseline vs. the O((n+k) log n) two-heap
// greedy using this repo's own Heap<T,TOrder>.
[MemoryDiagnoser]
public class IPOBenchmarks
{
    private const int K = 20;
    private const int RandomSeed = 11;
    private const int MaxProfit = 1_000;
    private const int MaxCapital = 1_000;

    private int[] _profits = [];

    private int[] _capitals = [];
    [Params(200, 5_000)]
    public int ProjectCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _profits = Enumerable.Range(0, ProjectCount).Select(_ => random.Next(1, MaxProfit)).ToArray();
        _capitals = Enumerable.Range(0, ProjectCount).Select(_ => random.Next(0, MaxCapital)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerRound() => IPOSolution.FindMaximizedCapitalByLinearScan(K, 0, _profits, _capitals);

    [Benchmark]
    public int TwoHeapGreedy() => IPOSolution.FindMaximizedCapitalByTwoHeapGreedy(K, 0, _profits, _capitals);
}
