using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TallestBillboard;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TallestBillboardSolution's, the same methods
// TallestBillboardTests proves correct. The rod set is generated once in [GlobalSetup],
// so what is measured is the recursion itself - 3^N un-memoized calls against Memoizer's
// one visit per (index, diff) state. N is kept modest specifically because the
// un-memoized baseline's 3^N blowup is real, the same reasoning TargetSumBenchmarks
// documents for its own 2^N baseline.
[MemoryDiagnoser]
public class TallestBillboardBenchmarks
{
    private const int RandomSeed = 956; // LC problem number
    private const int MaxRodLength = 50; private int[] _rods = [];

    // exclusive upper bound passed to Random.Next

    [Params(12, 14)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rods = Enumerable.Range(0, N).Select(_ => random.Next(1, MaxRodLength)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => TallestBillboardSolution.MaxHeightByUnmemoizedRecursion(_rods);

    [Benchmark]
    public int MemoizedRecursion() => TallestBillboardSolution.MaxHeightByMemoizedDiff(_rods);
}
