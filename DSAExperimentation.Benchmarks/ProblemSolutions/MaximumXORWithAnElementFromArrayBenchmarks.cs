using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumXORWithAnElementFromArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumXORWithAnElementFromArraySolution's, the same
// methods MaximumXORWithAnElementFromArrayTests proves correct. Values are spread
// over a range far wider than the query count so limits genuinely partition nums,
// and there are as many queries as elements - which is where the O(n*q) per-query
// scan and the O((n + q) log(n + q)) offline sweep actually diverge.
[MemoryDiagnoser]
public class MaximumXORWithAnElementFromArrayBenchmarks
{
    private const int RandomSeed = 1707;
    private const int ValueLimit = 1_000_000;

    [Params(200, 3_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueLimit)).ToArray();
        _queries = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(0, ValueLimit), random.Next(0, ValueLimit) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] LinearScanPerQuery() =>
        MaximumXORWithAnElementFromArraySolution.MaximizeXorByLinearScanPerQuery(_nums, _queries);

    [Benchmark]
    public int[] OfflineSortedBitTrieSweep() =>
        MaximumXORWithAnElementFromArraySolution.MaximizeXorByOfflineBitTrieSweep(_nums, _queries);
}
